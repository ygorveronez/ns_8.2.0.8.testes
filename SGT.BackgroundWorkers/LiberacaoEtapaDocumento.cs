using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class LiberacaoEtapaDocumento : LongRunningProcessBase<LiberacaoEtapaDocumento>
    {
        #region Metodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarCargasLiberarSemNFe(unitOfWork, _tipoServicoMultisoftware);
            VerificarCargasLIberarParaFaturamento(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);
        }

        #endregion

        private void VerificarCargasLIberarParaFaturamento(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            try
            {
                unitOfWork.FlushAndClear();

                Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Servicos.Embarcador.Logistica.PrazoSituacaoCarga servicoPrazoSituacaoCarga = new Servicos.Embarcador.Logistica.PrazoSituacaoCarga(unitOfWork);
                Servicos.Embarcador.NFSe.NFSe servicoNFSe = new Servicos.Embarcador.NFSe.NFSe(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga prazoSituacaoCarga = servicoPrazoSituacaoCarga.ObterPrazoSituacaoCarga(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamento.LiberarAutomaticamenteFaturamento);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamentoLiberarFaturamento = repCargaJanelaCarregamento.BuscarCargaLiberdasPeloTransportador(configuracaoTMS.SituacaoCargaJanelaLiberarFaturamento, DateTime.Now);

                if (prazoSituacaoCarga != null)
                {
                    DateTime prazoLimite = DateTime.Now.AddMinutes(prazoSituacaoCarga.Tempo);
                    cargasJanelaCarregamentoLiberarFaturamento.AddRange(repCargaJanelaCarregamento.BuscarPorSituacaoEPrazo(configuracaoTMS.SituacaoCargaJanelaLiberarFaturamento, prazoLimite));
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargasLiberadasParaFaturamento in cargasJanelaCarregamentoLiberarFaturamento.Distinct())
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargasLiberadasParaFaturamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Entrega);
                    bool liberada = true;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (cargaPedido.PossuiNFS)
                        {
                            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe configNFSe = servicoNFSe.BuscarConfiguracaoEmissaoNFSe(cargasLiberadasParaFaturamento.Carga.Empresa.Codigo, cargaPedido.Destino.Codigo, tomador?.Localidade?.Estado?.Sigla ?? "", tomador?.GrupoPessoas?.Codigo ?? 0, tomador?.Localidade?.Codigo ?? 0, cargasLiberadasParaFaturamento.Carga.TipoOperacao?.Codigo ?? 0, tomador?.CPF_CNPJ ?? 0, 0, unitOfWork);

                            if (configNFSe == null)
                            {
                                liberada = false;
                                break;
                            }
                        }

                        if (cargaPedido.SituacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.Nova)
                        {
                            cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF;
                            repCargaPedido.Atualizar(cargaPedido);
                        }
                    }

                    if (liberada)
                    {

                        if (!cargasLiberadasParaFaturamento.Carga.ExigeNotaFiscalParaCalcularFrete)
                            Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.ValidarCompraValePedagioComIntegracaoValorPedagioEmbarcador(cargasLiberadasParaFaturamento.Carga, unitOfWork, stringConexao, configuracaoTMS, tipoServicoMultisoftware);


                        bool ciotLiberado = true;
                        string retornoCIOT = Servicos.Embarcador.CIOT.CIOT.ObterCIOTCarga(cargasLiberadasParaFaturamento.Carga, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);
                        if (!string.IsNullOrWhiteSpace(retornoCIOT))
                            ciotLiberado = false;

                        if (ciotLiberado)
                        {
                            bool averbacaoLiberada = true;

                            if (configuracaoTMS.NaoPermiteEmitirCargaSemAverbacao && !(cargasLiberadasParaFaturamento.Carga.TipoOperacao?.FretePorContadoCliente ?? false))
                            {
                                averbacaoLiberada = false;
                                Dominio.Entidades.Empresa empresa = cargasLiberadasParaFaturamento.Carga.ObterEmpresaEmissora;

                                if (empresa == null || !empresa.LiberarEmissaoSemAverbacao)
                                    averbacaoLiberada = cargasLiberadasParaFaturamento.Carga.DadosSumarizados?.PossuiAverbacaoCTe ?? false;
                            }
                            if (averbacaoLiberada)
                            {
                                if (!cargasLiberadasParaFaturamento.Carga.ExigeNotaFiscalParaCalcularFrete)
                                {
                                    cargasLiberadasParaFaturamento.Carga.SituacaoCarga = configuracaoTMS.SituacaoCargaAposLiberarParaFaturamento;


                                    if (cargasLiberadasParaFaturamento.Carga.SituacaoCarga == SituacaoCarga.AgNFe)
                                        Servicos.Embarcador.Carga.CargaIntegracao.AdicionarIntegracoesEtapaNFe(cargasLiberadasParaFaturamento.Carga, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);

                                    if (cargasLiberadasParaFaturamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao)
                                    {
                                        cargasLiberadasParaFaturamento.Carga.GerandoIntegracoes = true;
                                        Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarIntegracoesCarga(cargasLiberadasParaFaturamento.Carga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, false);
                                    }

                                    if (cargasLiberadasParaFaturamento.Carga.DataEnvioUltimaNFe.HasValue)
                                    {
                                        cargasLiberadasParaFaturamento.Carga.DataEnvioUltimaNFe = DateTime.Now;
                                        cargasLiberadasParaFaturamento.Carga.DataInicioEmissaoDocumentos = DateTime.Now;

                                        if (cargasLiberadasParaFaturamento.Carga.Carregamento != null)
                                        {
                                            cargasLiberadasParaFaturamento.Carga.Carregamento.SituacaoCarregamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Fechado;
                                            repCarregamento.Atualizar(cargasLiberadasParaFaturamento.Carga.Carregamento);
                                        }
                                    }
                                }
                                else
                                {
                                    cargasLiberadasParaFaturamento.Carga.DataInicioEmissaoDocumentos = DateTime.Now;
                                    cargasLiberadasParaFaturamento.Carga.DataEnvioUltimaNFe = DateTime.Now;

                                    if (cargasLiberadasParaFaturamento.Carga.Carregamento != null)
                                    {
                                        cargasLiberadasParaFaturamento.Carga.Carregamento.SituacaoCarregamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Fechado;
                                        repCarregamento.Atualizar(cargasLiberadasParaFaturamento.Carga.Carregamento);
                                    }
                                }
                                servicoHubCarga.InformarCargaAtualizada(cargasLiberadasParaFaturamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao);
                            }
                            else
                                cargasLiberadasParaFaturamento.Carga.problemaAverbacaoCTe = true;
                        }

                        repCarga.Atualizar(cargasLiberadasParaFaturamento.Carga);
                    }
                }

                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        private void VerificarCargasLiberarSemNFe(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            unitOfWork.FlushAndClear();

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal servicoCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, IdentificadorControlePosicaoThread.VerificarCargasLiberarSemNFe);

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            List<int> codigosCargas = servicoOrquestradorFila.Ordenar((limiteRegistros) => repositorioCarga.BuscaCargasLiberarSemNFe(limiteRegistros));

            foreach (int codigoCarga in codigosCargas)
            {
                unitOfWork.Start();

                try
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(codigoCarga);

                    if (carga.TipoOperacao?.GerarDocumentoPadraoParaCadaPedidoCarga ?? false)
                    {
                        List<int> codigosCargaPedidoComDocumentoPadrao = repositorioPedidoXMLNotaFiscal.BuscarCodigosCargaPedidoPorCargaETipoDocumento(codigoCarga, TipoDocumento.Outros);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                        {
                            if (codigosCargaPedidoComDocumentoPadrao.Contains(cargaPedido.Codigo))
                                continue;

                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal()
                            {
                                CNPJTranposrtador = "",
                                DataEmissao = DateTime.Now,
                                Descricao = "Outros",
                                Destinatario = cargaPedido.Pedido.Destinatario ?? new Dominio.Entidades.Cliente(),
                                Emitente = cargaPedido.Pedido.Remetente ?? new Dominio.Entidades.Cliente(),
                                Modelo = "99",
                                nfAtiva = true,
                                Numero = 1,
                                PlacaVeiculoNotaFiscal = "",
                                Serie = "",
                                TipoDocumento = TipoDocumento.Outros,
                                TipoOperacaoNotaFiscal = TipoOperacaoNotaFiscal.Saida,
                                XML = "",
                                DataRecebimento = DateTime.Now,
                                Peso = 1m,
                                Valor = 1m
                            };

                            repositorioXMLNotaFiscal.Inserir(xmlNotaFiscal);
                            servicoCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoEmbarcador, false, out bool alteradoTipoDeCarga, _auditado);
                        }

                        carga.ProcessandoDocumentosFiscais = true;

                        repositorioCarga.Atualizar(carga);
                    }
                    else
                        servicoCarga.LiberarCargaSemNFe(carga, cargaPedidos, configuracaoEmbarcador, unitOfWork, tipoServicoMultisoftware);

                    servicoOrquestradorFila.RegistroLiberadoComSucesso(codigoCarga);

                    unitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao);

                    servicoOrquestradorFila.RegistroComFalha(codigoCarga, excecao.Message);
                }
            }
        }
    }
}