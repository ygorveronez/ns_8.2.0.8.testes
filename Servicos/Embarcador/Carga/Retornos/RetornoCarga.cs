using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.Retornos
{
    public class RetornoCarga
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public RetornoCarga(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void CriarRetornoColetaBackhaul(Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga)
        {
            if (!retornoCarga.TipoRetornoCarga.IsGerarCargaColetaBackhaul())
                return;

            Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul repositorioRetornoCargaColetaBackhaul = new Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul RetornoCargaColetaBackhaul = repositorioRetornoCargaColetaBackhaul.BuscarPorRetornoCarga(retornoCarga.Codigo);

            if (RetornoCargaColetaBackhaul == null)
            {
                RetornoCargaColetaBackhaul = new Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul()
                {
                    RetornoCarga = retornoCarga,
                    Situacao = SituacaoRetornoCargaColetaBackhaul.AguardandoGerarCarga
                };

                repositorioRetornoCargaColetaBackhaul.Inserir(RetornoCargaColetaBackhaul);
            }

            retornoCarga.SituacaoRetornoCarga = SituacaoRetornoCarga.GerandoCargaRetornoColetaBackhaul;
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido ObterPedidoBaseRetorno(Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga, Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosOrdenados = cargaAntiga.Pedidos.OrderBy(o => o.OrdemEntrega).Select(o => o.Pedido).ToList();

            int codigoPedidoBaseRetorno = 0;

            if (retornoCarga.OrigemRetorno == null)
                codigoPedidoBaseRetorno = pedidosOrdenados.LastOrDefault().Codigo;
            else
                codigoPedidoBaseRetorno = pedidosOrdenados.Where(o => o.Destinatario.Codigo == retornoCarga.OrigemRetorno.Codigo).LastOrDefault()?.Codigo ?? 0;

            return repositorioPedido.BuscarPorCodigo(codigoPedidoBaseRetorno) ?? throw new ServicoException("Não foi possível encontrar o pedido base para retorno");
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga GerarCarga(Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, bool cargaDeColeta, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Log.TratarErro($"Iniciou Carga retorno {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRetornos");

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repositorioPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFronteira repCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(_unitOfWork);

            WebService.Carga.Pedido servicoWSPedido = new WebService.Carga.Pedido(_unitOfWork);
            Carga servicoCarga = new Carga(_unitOfWork);
            CargaMotorista servicoCargaMotorista = new CargaMotorista(_unitOfWork);
            CargaPedido servicoCargaPedido = new CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga = repositorioCarga.BuscarPorCodigo(retornoCarga.Carga.Codigo);
            Dominio.Entidades.Empresa empresa = cargaAntiga.Empresa;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = retornoCarga.TipoRetornoCarga.TipoOperacao;

            if (cargaDeColeta)
                tipoOperacao = retornoCarga.TipoRetornoCarga.TipoOperacaoCargaColeta;

            if (tipoOperacao == null)
                tipoOperacao = cargaAntiga.TipoOperacao;

            Log.TratarErro($"Iniciou Criar Carga {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRetornos");

            Dominio.Entidades.Embarcador.Cargas.Carga cargaRetorno = new Dominio.Entidades.Embarcador.Cargas.Carga()
            {
                AgConfirmacaoUtilizacaoCredito = cargaAntiga.AgConfirmacaoUtilizacaoCredito,
                AgImportacaoCTe = false,
                AgImportacaoMDFe = false,
                AguardandoEmissaoDocumentoAnterior = false,
                AutorizouTodosCTes = false,
                CargaCancelamento = cargaAntiga.CargaCancelamento,
                CargaFechada = true,
                CargaIntegradaEmbarcador = cargaAntiga.CargaIntegradaEmbarcador,
                CargaTransbordo = cargaAntiga.CargaTransbordo,
                CodigoCargaEmbarcador = cargaAntiga.CodigoCargaEmbarcador,
                ControlaTempoParaEmissao = cargaAntiga.ControlaTempoParaEmissao,
                DataCarregamentoCarga = cargaAntiga.DataCarregamentoCarga,
                DataEnvioUltimaNFe = null,
                DataInicioEmissaoDocumentos = null,
                DataFinalPrevisaoCarregamento = cargaAntiga.DataFinalPrevisaoCarregamento,
                DataInicialPrevisaoCarregamento = cargaAntiga.DataInicialPrevisaoCarregamento,
                DataPrevisaoTerminoCarga = cargaAntiga.DataPrevisaoTerminoCarga,
                Empresa = empresa,
                ExigeNotaFiscalParaCalcularFrete = tipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? cargaAntiga.ExigeNotaFiscalParaCalcularFrete,
                Filial = cargaAntiga.Filial,
                FreteDeTerceiro = cargaAntiga.FreteDeTerceiro,
                GrupoPessoaPrincipal = cargaAntiga.GrupoPessoaPrincipal,
                MotivoPendencia = cargaAntiga.MotivoPendencia,
                MotivoPendenciaFrete = cargaAntiga.MotivoPendenciaFrete,
                NaoExigeVeiculoParaEmissao = tipoOperacao?.NaoExigeVeiculoParaEmissao ?? cargaAntiga.NaoExigeVeiculoParaEmissao,
                Operador = cargaAntiga.Operador,
                PossuiPendencia = cargaAntiga.PossuiPendencia,
                PrioridadeEnvioIntegracao = cargaAntiga.PrioridadeEnvioIntegracao,
                problemaCTE = cargaAntiga.problemaCTE,
                problemaAverbacaoCTe = cargaAntiga.problemaAverbacaoCTe,
                problemaMDFe = cargaAntiga.problemaMDFe,
                NaoGerarMDFe = tipoOperacao?.NaoEmitirMDFe ?? cargaAntiga.NaoGerarMDFe,
                problemaNFS = cargaAntiga.problemaNFS,
                SegmentoGrupoPessoas = cargaAntiga.SegmentoGrupoPessoas,
                SegmentoModeloVeicularCarga = cargaAntiga.SegmentoModeloVeicularCarga,
                SituacaoCarga = SituacaoCarga.Nova,
                TabelaFrete = null,
                TipoDeCarga = cargaAntiga.TipoDeCarga,
                CalcularFreteLote = cargaAntiga.CalcularFreteLote,
                ModeloVeicularCarga = cargaAntiga.ModeloVeicularCarga,
                TipoFreteEscolhido = TipoFreteEscolhido.Tabela,
                TipoOperacao = tipoOperacao,
                ValorFrete = 0,
                ValorFreteAPagar = 0,
                ValorFreteEmbarcador = 0,
                ValorFreteLeilao = 0,
                ValorFreteLiquido = 0,
                ValorFreteOperador = 0,
                ValorFreteTabelaFrete = 0,
                ValorICMS = 0,
                ValorISS = 0,
                ValorRetencaoISS = 0m,
                VeiculoIntegradoEmbarcador = cargaAntiga.VeiculoIntegradoEmbarcador,
                Veiculo = cargaAntiga.Veiculo,
                LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = false,
                EmpresaFilialEmissora = null
            };

            repositorioCarga.Inserir(cargaRetorno);

            string origemAuditoria = cargaAntiga.TipoOperacao?.ConfiguracaoCarga?.GerarRetornoAutomaticoMomento.ObterDescricao() ?? "confirmar entrega";
            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaRetorno, null, $"Carga de Retorno Gerada ao {origemAuditoria}", _unitOfWork);

            // Replicar as fronteiras da carga antiga pra nova
            CargaFronteira serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteirasCargaAntiga = serCargaFronteira.ObterFronteirasPorCarga(cargaAntiga);
            repCargaFronteira.CopiarFronteirasParaCarga(fronteirasCargaAntiga, cargaRetorno);

            cargaRetorno.Protocolo = cargaRetorno.Codigo;
            cargaRetorno.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

            foreach (Dominio.Entidades.Veiculo reboque in retornoCarga.Reboques.ToList())
                cargaRetorno.VeiculosVinculados.Add(reboque);

            servicoCargaMotorista.AdicionarMotoristas(cargaAntiga, cargaRetorno);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = ObterPedidoBaseRetorno(retornoCarga, cargaAntiga);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoRetorno = pedido.Clonar();

            pedido.ControleNumeracao = pedido.Codigo;
            repositorioPedido.Atualizar(pedido);

            Dominio.Entidades.Cliente remetente = pedido.Destinatario;
            Dominio.Entidades.Cliente destinatario = pedido.Remetente;

            if (retornoCarga.ClienteColeta != null && !cargaDeColeta)
                destinatario = retornoCarga.ClienteColeta;
            else if (cargaDeColeta && retornoCarga.ClienteColeta != null)
            {
                remetente = retornoCarga.ClienteColeta;
                destinatario = pedido.Remetente;
            }

            Utilidades.Object.DefinirListasGenericasComoNulas(pedidoRetorno);
            pedidoRetorno.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
            pedidoRetorno.Remetente = remetente;
            Random rand = new Random();
            pedidoRetorno.ControleNumeracao = rand.Next();
            pedidoRetorno.Destinatario = destinatario;
            pedidoRetorno.EnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
            pedidoRetorno.EnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();

            servicoWSPedido.PreecherEnderecoPedidoPorCliente(pedidoRetorno.EnderecoOrigem, remetente);
            servicoWSPedido.PreecherEnderecoPedidoPorCliente(pedidoRetorno.EnderecoDestino, destinatario);

            repositorioPedidoEndereco.Inserir(pedidoRetorno.EnderecoOrigem);
            repositorioPedidoEndereco.Inserir(pedidoRetorno.EnderecoDestino);

            pedidoRetorno.Origem = remetente.Localidade;
            pedidoRetorno.Destino = destinatario.Localidade;
            pedidoRetorno.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
            repositorioPedido.Inserir(pedidoRetorno);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCargaEPedido(cargaAntiga.Codigo, pedido.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRetorno = cargaPedido.Clonar();

            Utilidades.Object.DefinirListasGenericasComoNulas(cargaPedidoRetorno);

            cargaPedidoRetorno.Carga = cargaRetorno;
            cargaPedidoRetorno.CargaOrigem = cargaRetorno;
            cargaPedidoRetorno.Pedido = pedidoRetorno;
            cargaPedidoRetorno.Recebedor = cargaPedido.Expedidor;
            cargaPedidoRetorno.Expedidor = cargaPedido.Recebedor;
            cargaPedidoRetorno.ModeloDocumentoFiscal = tipoOperacao?.ModeloDocumentoFiscal;
            cargaPedidoRetorno.CargaPedidoFilialEmissora = false;
            cargaPedidoRetorno.TipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.Normal;
            cargaPedidoRetorno.TipoContratacaoCarga = TipoContratacaoCarga.Normal;
            cargaPedidoRetorno.RegraTomador = null;

            if (retornoCarga.DataPrevisaoEntrega != null)
                retornoCarga.DataPrevisaoEntrega = cargaPedidoRetorno.Pedido.PrevisaoEntrega;

            bool cargaDeComplemento = false;
            if (tipoOperacao != null && tipoOperacao.GerarCTeComplementarNaCarga)
            {
                cargaPedidoRetorno.TipoContratacaoCarga = TipoContratacaoCarga.Redespacho;
                cargaDeComplemento = true;

                if (cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                    cargaPedidoRetorno.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                else if (cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                    cargaPedidoRetorno.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;

                if (retornoCarga.ClienteColeta != null)
                {
                    if (cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        cargaPedidoRetorno.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                    else
                    {
                        cargaPedidoRetorno.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                        cargaPedidoRetorno.Tomador = cargaPedido.ObterTomador();
                    }
                }
            }

            cargaPedidoRetorno.Origem = cargaPedidoRetorno.Expedidor != null ? cargaPedidoRetorno.Expedidor.Localidade : remetente.Localidade;
            cargaPedidoRetorno.Destino = cargaPedidoRetorno.Recebedor != null ? cargaPedidoRetorno.Recebedor.Localidade : destinatario.Localidade;
            cargaPedidoRetorno.ValorFrete = 0;
            cargaPedidoRetorno.PendenteGerarCargaDistribuidor = false;
            cargaPedidoRetorno.ValorFreteAPagar = 0;
            cargaPedidoRetorno.CargaRedespacho = null;
            cargaPedidoRetorno.ValorFreteAPagarFilialEmissora = 0;
            cargaPedidoRetorno.ValorFreteTabelaFrete = 0;
            cargaPedidoRetorno.ValorFreteTabelaFreteFilialEmissora = 0;
            cargaPedidoRetorno.ImpostoInformadoPeloEmbarcador = false;
            cargaPedidoRetorno.ValorFreteFilialEmissora = 0;
            cargaPedidoRetorno.BaseCalculoICMS = 0;
            cargaPedidoRetorno.CargaPedidoProximoTrecho = null;
            cargaPedidoRetorno.CargaPedidoTrechoAnterior = null;
            cargaPedidoRetorno.ValorICMS = 0;
            cargaPedidoRetorno.ValorAdValorem = 0;
            cargaPedidoRetorno.ValorDescarga = 0;

            bool possuiCTe = false;
            bool possuiNFS = false;
            bool possuiNFSManual = false;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoIntramunicipal = null;

            if (!cargaDeComplemento)
            {
                servicoCargaPedido.VerificarQuaisDocumentosDeveEmitir(cargaRetorno, cargaPedidoRetorno, cargaPedidoRetorno.Origem, cargaPedidoRetorno.Destino, tipoServicoMultisoftware, _unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoIntramunicipal, configuracaoTMS, out bool sempreDisponibilizarDocumentoNFSManual);

                cargaPedidoRetorno.DisponibilizarDocumentoNFSManual = sempreDisponibilizarDocumentoNFSManual;
                cargaPedidoRetorno.PossuiCTe = possuiCTe;
                cargaPedidoRetorno.PossuiNFS = possuiNFS;
                cargaPedidoRetorno.PossuiNFSManual = possuiNFSManual;
            }
            else
            {
                cargaPedidoRetorno.PossuiCTe = cargaPedido.PossuiCTe;
                cargaPedidoRetorno.PossuiNFS = cargaPedido.PossuiNFS;
                cargaPedidoRetorno.PossuiNFSManual = cargaPedido.PossuiNFSManual;
            }

            repositorioCargaPedido.Inserir(cargaPedidoRetorno);
            repositorioCarga.Atualizar(cargaRetorno);

            if (cargaDeComplemento || cargaDeColeta)
            {
                if (cargaPedidoRetorno.PossuiCTe && !cargaDeColeta)
                {
                    FilialEmissora serFilialEmissora = new FilialEmissora();
                    cargaPedidoRetorno.ImpostoInformadoPeloEmbarcador = true;
                    cargaRetorno.EmitirCTeComplementar = true;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargapedidoAnterior in cargaAntiga.Pedidos.ToList())
                        serFilialEmissora.GerarCTesAnterioresDaFilialEmissoraRetorno(cargapedidoAnterior, cargaPedidoRetorno, tipoServicoMultisoftware, configuracaoTMS, _unitOfWork, false);
                }
                else
                {
                    cargaPedidoRetorno.TipoContratacaoCarga = TipoContratacaoCarga.Normal;
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidosXMLNotaFiscal in pedidosXMLNotasFiscais)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalClone = pedidosXMLNotaFiscal.Clonar();

                        pedidoXMLNotaFiscalClone.ValorFrete = 0;
                        pedidoXMLNotaFiscalClone.ValorFreteFilialEmissora = 0;
                        pedidoXMLNotaFiscalClone.ValorFreteTabelaFrete = 0;
                        pedidoXMLNotaFiscalClone.ValorFreteTabelaFreteFilialEmissora = 0;
                        pedidoXMLNotaFiscalClone.BaseCalculoICMS = 0;
                        pedidoXMLNotaFiscalClone.ValorICMS = 0;
                        pedidoXMLNotaFiscalClone.TipoNotaFiscal = TipoNotaFiscal.Venda;
                        pedidoXMLNotaFiscalClone.CargaPedido = cargaPedidoRetorno;

                        Utilidades.Object.DefinirListasGenericasComoNulas(pedidoXMLNotaFiscalClone);

                        repositorioPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscalClone);
                    }
                }
                cargaRetorno.DataInicioEmissaoDocumentos = DateTime.Now;
                cargaRetorno.DataEnvioUltimaNFe = DateTime.Now;
                repositorioCarga.Atualizar(cargaRetorno);
            }

            servicoCarga.FecharCarga(cargaRetorno, _unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware, true);
            Servicos.Log.TratarErro("9 - Fechou Carga (" + cargaRetorno.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");

            return cargaRetorno;
        }

        private void GerarCargasRetorno(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga> retornosCarga = repositorioRetornoCarga.BuscarPorSituacao(SituacaoRetornoCarga.GerandoCargaRetorno, limite: 2);

            foreach (Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga in retornosCarga)
            {
                _unitOfWork.Start();

                retornoCarga.CargaRetorno = GerarCarga(retornoCarga, tipoServicoMultisoftware, configuracaoTMS, ClienteMultisoftware, cargaDeColeta: false, Auditado);
                retornoCarga.SituacaoRetornoCarga = SituacaoRetornoCarga.RetornoGerado;

                CriarRetornoColetaBackhaul(retornoCarga);

                repositorioRetornoCarga.Atualizar(retornoCarga);

                _unitOfWork.CommitChanges();
            }
        }

        private void GerarCargasRetornoColetaBackhaul(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul repositorioRetornoCargaColetaBackhaul = new Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul> retornosCargaColetaBackhaul = repositorioRetornoCargaColetaBackhaul.BuscarPorSituacao(SituacaoRetornoCargaColetaBackhaul.GerandoCarga, limite: 2);

            foreach (Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul retornoCargaColetaBackhaul in retornosCargaColetaBackhaul)
            {
                _unitOfWork.Start();

                retornoCargaColetaBackhaul.RetornoCarga.CargaColeta = GerarCarga(retornoCargaColetaBackhaul.RetornoCarga, tipoServicoMultisoftware, configuracaoTMS, null, cargaDeColeta: true, Auditado);
                retornoCargaColetaBackhaul.RetornoCarga.SituacaoRetornoCarga = SituacaoRetornoCarga.RetornoGerado;
                retornoCargaColetaBackhaul.Situacao = SituacaoRetornoCargaColetaBackhaul.CargaGerada;

                repositorioRetornoCarga.Atualizar(retornoCargaColetaBackhaul.RetornoCarga);
                repositorioRetornoCargaColetaBackhaul.Atualizar(retornoCargaColetaBackhaul);

                _unitOfWork.CommitChanges();
            }
        }

        #endregion

        #region Métodos Públicos

        public void CriarRetorno(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!(carga.TipoOperacao?.OperacaoExigeInformarCargaRetorno ?? false))
                return;

            Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioCargaRetorno = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga = repositorioCargaRetorno.BuscarPorCarga(carga.Codigo);

            if (retornoCarga == null)
            {
                retornoCarga = new Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga()
                {
                    Carga = carga,
                    Reboques = carga.VeiculosVinculados.ToList(),
                    SituacaoRetornoCarga = SituacaoRetornoCarga.AgInformarRetorno
                };

                repositorioCargaRetorno.Inserir(retornoCarga);
            }
        }

        public void DisponibilizarCargaCanceladaParaNovoRetorno(Dominio.Entidades.Embarcador.Cargas.Carga cargaCancelada)
        {
            Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            if (cargaCancelada.CargaAgrupada)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                cargas = repCarga.BuscarCargasOriginais(cargaCancelada.Codigo);
            }
            else
                cargas.Add(cargaCancelada);


            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga = repositorioRetornoCarga.BuscarPorCargaRetorno(carga.Codigo);

                if (retornoCarga != null)
                {
                    retornoCarga.CargaRetorno = null;
                    retornoCarga.SituacaoRetornoCarga = SituacaoRetornoCarga.AgInformarRetorno;
                    retornoCarga.TipoRetornoCarga = null;
                    retornoCarga.Reboques = retornoCarga.Carga.VeiculosVinculados.ToList();
                    retornoCarga.ClienteColeta = null;

                    repositorioRetornoCarga.Atualizar(retornoCarga);
                }
                else
                {
                    Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul repositorioRetornoCargaColetaBackhaul = new Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul(_unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul retornoCargaColetaBackhaul = repositorioRetornoCargaColetaBackhaul.BuscarPorCargaColetaBackhaul(carga.Codigo);

                    if (retornoCargaColetaBackhaul != null)
                    {
                        retornoCargaColetaBackhaul.RetornoCarga.CargaColeta = null;

                        repositorioRetornoCarga.Atualizar(retornoCargaColetaBackhaul.RetornoCarga);
                        repositorioRetornoCargaColetaBackhaul.Deletar(retornoCargaColetaBackhaul);
                    }
                }
            }
        }

        public string ValidarCargaCanceladaParaNovoRetorno(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga = repositorioRetornoCarga.BuscarPorCargaRetorno(carga.Codigo);

            if (retornoCarga?.CargaColeta != null)
                return "Não é possível cancelar a carga pois existe uma viagem de coleta gerada para a mesma, por favor, cancele a viagem de coleta primeiro para depois cancelar a carga de retorno.";

            return string.Empty;
        }

        public void VerificarCargasPendenteRetorno(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            _unitOfWork.FlushAndClear();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            GerarCargasRetorno(configuracaoTMS, tipoServicoMultisoftware, ClienteMultisoftware, Auditado);
            GerarCargasRetornoColetaBackhaul(configuracaoTMS, tipoServicoMultisoftware, Auditado);
        }

        public void SolicitarGeracaoCargaRetorno(Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga, int codigoTipoRetornoCarga, double codigoPontosColeta, int codigoReboque, int codigoSegundoReboque, double codigoClienteColeta, bool retornoSomenteComTracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorioTipoRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(_unitOfWork);

            if (retornoCarga.SituacaoRetornoCarga != SituacaoRetornoCarga.AgInformarRetorno && !(retornoCarga.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoChamado?.NaoValidarRetornoGeradoParaFinalizacaoAtendimento ?? false))
                throw new ServicoException(string.Format(Localization.Resources.Cargas.RetornoCarga.NaoFoiPossivelGerarRetornoCargaAtual, retornoCarga.SituacaoRetornoCarga.ObterDescricao()));

            _unitOfWork.Start();

            retornoCarga.Reboques.Clear();
            retornoCarga.Reboques = new List<Dominio.Entidades.Veiculo>();
            retornoCarga.RetornarSomenteComTracao = retornoSomenteComTracao;
            retornoCarga.SituacaoRetornoCarga = SituacaoRetornoCarga.GerandoCargaRetorno;
            retornoCarga.OrigemRetorno = codigoPontosColeta > 0 ? repositorioCliente.BuscarPorCPFCNPJ(codigoPontosColeta) : null;
            if (retornoCarga.TipoRetornoCarga == null)
                retornoCarga.TipoRetornoCarga = codigoTipoRetornoCarga > 0 ? repositorioTipoRetornoCarga.BuscarPorCodigo(codigoTipoRetornoCarga) : null;

            if (!retornoCarga.RetornarSomenteComTracao)
            {
                int numeroReboques = retornoCarga.Carga.ModeloVeicularCarga?.NumeroReboques ?? 0;
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);

                if (codigoReboque > 0)
                {
                    Dominio.Entidades.Veiculo reboque = repositorioVeiculo.BuscarPorCodigo(codigoReboque);
                    retornoCarga.Reboques.Add(reboque);
                }

                if (codigoSegundoReboque > 0)
                {
                    Dominio.Entidades.Veiculo segundoReboque = repositorioVeiculo.BuscarPorCodigo(codigoSegundoReboque);
                    retornoCarga.Reboques.Add(segundoReboque);
                }

                //if (retornoCarga.Reboques.Count < numeroReboques) //esta estourando excessao ao finalizar viagem pelo monitoramento automaticamente, nao pode.
                //    throw new ControllerException(Localization.Resources.Cargas.RetornoCarga.NecessarioInformarRoboqueParaGerarRetorno);
            }

            if (retornoCarga.TipoRetornoCarga.ExigeClienteColeta)
                retornoCarga.ClienteColeta = repositorioCliente.BuscarPorCPFCNPJ(codigoClienteColeta);

            if (auditado != null)
                repositorioRetornoCarga.Atualizar(retornoCarga, auditado);
            else
                repositorioRetornoCarga.Atualizar(retornoCarga);

            _unitOfWork.CommitChanges();
        }

        public bool GerarCargaRetorno(Dominio.Entidades.Embarcador.Cargas.Carga carga, int codigoTipoRetornoCarga, double codigoPontosColeta, int codigoReboque, int codigoSegundoReboque, double codigoClienteColeta, bool retornoSomenteComTracao = false, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorioTipoRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga = repositorioRetornoCarga.BuscarPorCarga(carga.Codigo);

            if (retornoCarga == null)
            {
                retornoCarga = new Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga();
                retornoCarga.Carga = carga;
                retornoCarga.Reboques = carga.VeiculosVinculados?.ToList() ?? new List<Dominio.Entidades.Veiculo>();
                retornoCarga.SituacaoRetornoCarga = SituacaoRetornoCarga.AgInformarRetorno;
                retornoCarga.TipoRetornoCarga = carga.TipoOperacao?.TipoRetornoCarga ?? repositorioTipoRetornoCarga.BuscarPrimeiroRegistro();
                repositorioRetornoCarga.Inserir(retornoCarga);
            }

            SolicitarGeracaoCargaRetorno(retornoCarga, codigoTipoRetornoCarga, codigoPontosColeta, codigoReboque, codigoSegundoReboque, codigoClienteColeta, retornoSomenteComTracao, auditado);

            return true;
        }

        #endregion

    }
}
