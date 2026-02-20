using Dominio.Entidades.Embarcador.Cargas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Servicos.Embarcador.Integracao.Natura
{
    public class IntegracaoDTNatura : ServicoBase
    {
        #region Construtores
        
        public IntegracaoDTNatura(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Métodos Globais

        public bool VincularCargaAoDT(Dominio.Entidades.Usuario usuario, List<int> codigosDT, int codigoCargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho, out string mensagem)
        {
            Servicos.Embarcador.Carga.Frete svcFrete = new Servicos.Embarcador.Carga.Frete(unidadeDeTrabalho);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unidadeDeTrabalho);
            Servicos.Embarcador.Carga.CTe serCargaCTe = new Servicos.Embarcador.Carga.CTe(unidadeDeTrabalho);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unidadeDeTrabalho);
            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unidadeDeTrabalho);
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unidadeDeTrabalho);
            Servicos.Embarcador.Carga.RateioFormula serRateioFormula = new Carga.RateioFormula(unidadeDeTrabalho);
            Servicos.Embarcador.Carga.Rota serRota = new Servicos.Embarcador.Carga.Rota(unidadeDeTrabalho);

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDTNatura = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaIntegracaoNatura repCargaIntegracaoNatura = new Repositorio.Embarcador.Cargas.CargaIntegracaoNatura(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDTNatura = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unidadeDeTrabalho);


            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            if (cargaPedido == null)
            {
                mensagem = "Pedido não encontrado.";
                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

            bool geradoPorNOTFIS = false;
            bool podeAlterarCarga = carga.SituacaoCarga.IsSituacaoCargaNaoEmitida();

            foreach (int codigoDT in codigosDT)
            {
                Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura = repDTNatura.BuscarPorCodigo(codigoDT);
                List<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura> notasFiscaisDTNatura = repNotaFiscalDTNatura.BuscarPorDT(codigoDT);

                if (dtNatura == null)
                {
                    mensagem = "Documento de transporte da Natura não encontrado.";
                    return false;
                }

                if (dtNatura.Cargas.Where(obj => obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                                                 obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada).Any())
                {
                    mensagem = "O documento de transporte (" + dtNatura.Numero.ToString() + ") da Natura já está vinculado à carga " + dtNatura.Cargas[0].Carga.CodigoCargaEmbarcador + ".";
                    return false;
                }

                if (notasFiscaisDTNatura.Count <= 0)
                {
                    mensagem = "O documento de transporte (" + dtNatura.Numero.ToString() + ") da Natura não possui notas fiscais vinculadas.";
                    return false;
                }

                if (dtNatura.GeradoPorNOTFIS)
                    geradoPorNOTFIS = true;

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura cargaIntegracaoNatura = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura
                {
                    Carga = carga,
                    CargaPedido = cargaPedido,
                    DocumentoTransporte = dtNatura,
                    Usuario = usuario
                };

                repCargaIntegracaoNatura.Inserir(cargaIntegracaoNatura);

                if (podeAlterarCarga)
                {
                    if (!SalvarNotasFiscaisPedido(dtNatura, notasFiscaisDTNatura, cargaPedido, unidadeDeTrabalho, out mensagem, tipoServicoMultisoftware))
                        return false;
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXML = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo); //(from obj in carga.Pedidos select repPedidoXMLNotaFiscal.BuscarPorCargaPedido(obj.Codigo)).SelectMany(o => o).ToList();

                    foreach (Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura notaFiscalDT in notasFiscaisDTNatura)
                    {
                        if (!pedidosXML.Where(obj => obj.XMLNotaFiscal.Chave == notaFiscalDT.Chave || (obj.XMLNotaFiscal.Numero == notaFiscalDT.Numero && obj.XMLNotaFiscal.Serie == notaFiscalDT.Serie.ToString())).Any())
                        {
                            mensagem = "Os dados das notas fiscais (chave, número e série) diferem entre o documento de transporte e a carga.";
                            return false;
                        }
                    }
                }
            }

            if (podeAlterarCarga)
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidadePagamentoFrete = repPedidoXMLNotaFiscal.BuscarModalidadeDeFretePadraoPorCargaPedido(cargaPedido.Codigo);

                if (modalidadePagamentoFrete.HasValue && modalidadePagamentoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido)
                    cargaPedido.Pedido.TipoPagamento = (Dominio.Enumeradores.TipoPagamento)modalidadePagamentoFrete;

                if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Outros)
                {
                    if (cargaPedido.Tomador != null)
                        cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                    else
                        cargaPedido.Tomador = null;
                }
                else
                {
                    cargaPedido.Tomador = null;

                    if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                        cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                    else
                        cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> integracoesNatura = repCargaIntegracaoNatura.BuscarPorCargaPedido(cargaPedido.Codigo);

                decimal peso = repNotaFiscalDTNatura.BuscarPesoPorDT(integracoesNatura.Select(o => o.DocumentoTransporte.Codigo)); //integracoesNatura.Sum(o => o.DocumentoTransporte.NotasFiscais.Sum(n => n.Peso));
                int volumes = repNotaFiscalDTNatura.BuscarVolumesPorDT(integracoesNatura.Select(o => o.DocumentoTransporte.Codigo)); //integracoesNatura.Sum(o => o.DocumentoTransporte.NotasFiscais.Sum(n => n.Quantidade));

                if (peso > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades
                    {
                        CargaPedido = cargaPedido,
                        Quantidade = peso,
                        Unidade = Dominio.Enumeradores.UnidadeMedida.KG
                    };

                    cargaPedidoQuantidades.Add(cargaPedidoQuantidade);
                }

                if (volumes > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades
                    {
                        CargaPedido = cargaPedido,
                        Quantidade = volumes,
                        Unidade = Dominio.Enumeradores.UnidadeMedida.UN
                    };

                    cargaPedidoQuantidades.Add(cargaPedidoQuantidade);
                }

                serCargaPedido.AdicionarCargaPedidoQuantidades(cargaPedidoQuantidades, cargaPedido, unidadeDeTrabalho);

                //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {peso}. ImportacaoDTNatura.VincularCargaAoDT", "PesoCargaPedido");
                cargaPedido.Peso = peso;

                repPedido.Atualizar(cargaPedido.Pedido);
                repCargaPedido.Atualizar(cargaPedido);

                Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = serRateioFormula.ObterFormulaDeRateio(carga, unidadeDeTrabalho, cargaPedido);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos> tiposEmissaoCTeDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos>();
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes> tipoEmissaoCTeParticipantes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPed in cargaPedidos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumento = serCargaCTe.BuscarTipoEmissaoDocumentosCTe(cargaPed, tipoServicoMultisoftware, unidadeDeTrabalho);
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoParticipante = serCargaCTe.BuscarTipoEmissaoCTeParticipantes(cargaPed, tipoServicoMultisoftware, unidadeDeTrabalho, false);

                    if (!tiposEmissaoCTeDocumentos.Contains(tipoEmissaoCTeDocumento))
                        tiposEmissaoCTeDocumentos.Add(tipoEmissaoCTeDocumento);

                    if (!tipoEmissaoCTeParticipantes.Contains(tipoEmissaoParticipante))
                        tipoEmissaoCTeParticipantes.Add(tipoEmissaoParticipante);

                    cargaPed.FormulaRateio = formulaRateio;
                    cargaPed.TipoRateio = tipoEmissaoCTeDocumento;
                    cargaPed.TipoEmissaoCTeParticipantes = tipoEmissaoParticipante;

                    if (!geradoPorNOTFIS)
                    {
                        if (tipoEmissaoParticipante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor ||
                            tipoEmissaoParticipante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor && cargaPed.Recebedor == null)
                            cargaPed.Recebedor = cargaPedido.Pedido.Destinatario;

                        if (tipoEmissaoParticipante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor ||
                            tipoEmissaoParticipante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor && cargaPed.Expedidor == null)
                            cargaPed.Expedidor = cargaPedido.Pedido.Remetente;
                    }

                    repCargaPedido.Atualizar(cargaPed);
                }

                serRota.DeletarPercursoDestinosCarga(carga, unidadeDeTrabalho);

                //foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumento in tiposEmissaoCTeDocumentos)
                //{
                //    foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoParticipante in tipoEmissaoCTeParticipantes)
                //    {
                //        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTipoRateio = (from obj in cargaPedidos where obj.TipoRateio == tipoEmissaoCTeDocumento && obj.TipoEmissaoCTeParticipantes == tipoEmissaoParticipante select obj).ToList();
                //        if (cargaPedidosTipoRateio.Count > 0)
                serCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, cargaPedidos, unidadeDeTrabalho, tipoServicoMultisoftware, configuracaoPedido);
                //    }
                //}

                carga.ValorFreteEmbarcador = repCargaIntegracaoNatura.BuscarValorFretePorCargaPedido(cargaPedido.Codigo); //integracoesNatura.Sum(o => o.DocumentoTransporte.ValorFrete);
                carga.ValorFrete = carga.ValorFreteEmbarcador;
                carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                carga.PossuiPendencia = false;

                Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Embarcador", " Valor Informado = " + carga.ValorFrete.ToString("n2"), carga.ValorFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Embarcador", 0, carga.ValorFrete);
                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicaoFrete, unidadeDeTrabalho, null);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    serRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, false, unidadeDeTrabalho, tipoServicoMultisoftware);

                repCarga.Atualizar(carga);
            }

            mensagem = string.Empty;
            return true;
        }

        public static bool GerarCargaPorDT(Dominio.Entidades.Embarcador.Integracao.DTNatura documentoTransporteNatura, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, out int codigoCarga, out string msgErro)
        {
            msgErro = "";
            codigoCarga = 0;

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura nota = documentoTransporteNatura.NotasFiscais.FirstOrDefault();

            // Cria Pedido
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoPadrao = repTipoOperacao.BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracao();

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido()
            {
                Remetente = nota.Emitente,
                Destinatario = nota.Destinatario,
                TipoOperacao = tipoOperacaoPadrao,
                Filial = repFilial.BuscarPorCNPJ(nota.Emitente.CPF_CNPJ_SemFormato),
                TipoDeCarga = tipoOperacaoPadrao.TipoDeCargaPadraoOperacao,
                SituacaoPedido = SituacaoPedido.Aberto,
                Numero = repPedido.BuscarProximoNumero(),
                UltimaAtualizacao = DateTime.Now,
                Usuario = usuario,
                Autor = usuario,
                NumeroPedidoEmbarcador = documentoTransporteNatura.Numero.ToString(),
                Origem = nota.Emitente.Localidade,
                Destino = nota.Destinatario.Localidade,
                Empresa = documentoTransporteNatura.Empresa,
                GerarAutomaticamenteCargaDoPedido = true,
                Veiculos = new List<Dominio.Entidades.Veiculo>()
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (pedido.TipoOperacao != null && !pedido.TipoOperacao.GeraCargaAutomaticamente)
                {
                    pedido.GerarAutomaticamenteCargaDoPedido = false;
                    pedido.PedidoTotalmenteCarregado = false;
                }
            }

            pedido.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
            repPedido.Inserir(pedido);

            pedido.Protocolo = pedido.Codigo;

            // Gera Carga
            pedido.PedidoIntegradoEmbarcador = true;

            if (pedido.GerarAutomaticamenteCargaDoPedido)
            {
                if (!configuracaoEmbarcador.NumeroCargaSequencialUnico)
                    pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, pedido.Filial?.Codigo ?? 0).ToString();
                else
                    pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();

                pedido.AdicionadaManualmente = true;
            }

            msgErro = Servicos.Embarcador.Pedido.Pedido.CriarCarga(pedido, unitOfWork, TipoServicoMultisoftware, null, configuracaoEmbarcador);

            if (string.IsNullOrWhiteSpace(msgErro))
                repPedido.Atualizar(pedido);
            else
            {
                unitOfWork.Rollback();
                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorPedido(pedido.Codigo);
            codigoCarga = carga.Codigo;

            return true;
        }

        public void ConsultarDT(Dominio.Entidades.Usuario usuario, long numeroDT, DateTime dataInicial, DateTime dataFinal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            bool autorizaValorZerado = false;

            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.IntegracaoNatura repIntegracaoNatura = new Repositorio.Embarcador.Integracao.IntegracaoNatura(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDocumentoTransporte = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDTNatura = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.CodigoMatrizNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaNatura))
                throw new Exception("Os dados para a integração com a Natura não estão configurados.");

            ServicoNatura.ProcessaConsultaNF.SI_ProcessaConsultaNFSync_OBClient svcConsultaNF = ObterClientNatura<ServicoNatura.ProcessaConsultaNF.SI_ProcessaConsultaNFSync_OBClient, ServicoNatura.ProcessaConsultaNF.SI_ProcessaConsultaNFSync_OB>(configuracaoIntegracao.UsuarioNatura, configuracaoIntegracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_SI_ProcessaConsultaNF, unidadeDeTrabalho, out Servicos.Models.Integracao.InspectorBehavior inspectorProcessaConsultaNF);
            ServicoNatura.RecebeNotasFiscais.SI_RecebeNotaisFiscaisSync_OBClient svcRecebeNotasFiscais = ObterClientNatura<ServicoNatura.RecebeNotasFiscais.SI_RecebeNotaisFiscaisSync_OBClient, ServicoNatura.RecebeNotasFiscais.SI_RecebeNotaisFiscaisSync_OB>(configuracaoIntegracao.UsuarioNatura, configuracaoIntegracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_SI_RecebeNotaisFiscais, unidadeDeTrabalho, out Servicos.Models.Integracao.InspectorBehavior inspectorRecebeNotasFiscais);

            var dadosConsulta = new ServicoNatura.ProcessaConsultaNF.DT_EnviaParamConsultaNFDados() { codTranspMatriz = configuracaoIntegracao.CodigoMatrizNatura };

            if (numeroDT > 0)
            {
                dadosConsulta.numTransporte = numeroDT.ToString();
                autorizaValorZerado = true;
            }
            else
            {
                if (dataFinal != DateTime.MinValue)
                    dadosConsulta.dataAte = dataFinal.ToString("yyyy-MM-dd");

                if (dataInicial != DateTime.MinValue)
                    dadosConsulta.dataDe = dataInicial.ToString("yyyy-MM-dd");
            }

            if (svcConsultaNF.Endpoint.Address.ToString().Contains("qxx.transportes.natura.com.br") || svcRecebeNotasFiscais.Endpoint.Address.ToString().Contains("qxx.transportes.natura.com.br"))
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var retornoConsulta = svcConsultaNF.SI_ProcessaConsultaNFSync_OB(new ServicoNatura.ProcessaConsultaNF.DT_EnviaParamConsultaNF() { dados = dadosConsulta });

            Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura integracaoNatura = new Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura();

            integracaoNatura.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorProcessaConsultaNF.LastRequestXML, "xml", unidadeDeTrabalho);
            integracaoNatura.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorProcessaConsultaNF.LastResponseXML, "xml", unidadeDeTrabalho);
            integracaoNatura.DataConsulta = DateTime.Now;

            if (dataFinal != DateTime.MinValue)
                integracaoNatura.ParametroDataFinal = dataFinal;

            if (dataInicial != DateTime.MinValue)
                integracaoNatura.ParametroDataInicial = dataInicial;

            if (numeroDT > 0)
                integracaoNatura.ParametroNumero = numeroDT;

            integracaoNatura.Protocolo = retornoConsulta.dados.protocolo;
            integracaoNatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoNatura.Sucesso;
            integracaoNatura.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoNatura.DT;
            integracaoNatura.Usuario = usuario;
            integracaoNatura.Retorno = "Consulta de DTs realizada com sucesso.";
            integracaoNatura.SubIntegracoes = new List<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura>();

            repIntegracaoNatura.Inserir(integracaoNatura);

            ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDados[] retornoNotasFiscais = null;

            int countExec = 0;
            do
            {
                if (countExec > 0)
                    System.Threading.Thread.Sleep(1000);

                retornoNotasFiscais = svcRecebeNotasFiscais.SI_RecebeNotaisFiscaisSync_OB(new ServicoNatura.RecebeNotasFiscais.DT_EnviaParamNotasFiscais()
                {
                    dados = new ServicoNatura.RecebeNotasFiscais.DT_EnviaParamNotasFiscaisDados()
                    {
                        codTranspMatriz = configuracaoIntegracao.CodigoMatrizNatura,
                        protocolo = integracaoNatura.Protocolo
                    }
                });

                Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura subIntegracao = new Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura();

                subIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorRecebeNotasFiscais.LastRequestXML, "xml", unidadeDeTrabalho);
                subIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorRecebeNotasFiscais.LastResponseXML, "xml", unidadeDeTrabalho);
                subIntegracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoNatura.Sucesso;
                subIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoNatura.DT;
                subIntegracao.DataConsulta = DateTime.Now;

                string mensagem = retornoNotasFiscais[0]?.mensagem;
                subIntegracao.Retorno = string.IsNullOrWhiteSpace(mensagem) ? "Retorno realizado com sucesso" : mensagem;

                repIntegracaoNatura.Inserir(subIntegracao);

                integracaoNatura.SubIntegracoes.Add(subIntegracao);

                repIntegracaoNatura.Atualizar(integracaoNatura);

                countExec++;

            } while (retornoNotasFiscais[0].mensagem != null && retornoNotasFiscais[0].mensagem.ToLower() == "solicitação em processamento" && countExec < 50);


            foreach (ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDados retorno in retornoNotasFiscais) //para cada documento de transporte (DT)
            {
                if (string.IsNullOrWhiteSpace(retorno.documentoTransporte))
                    continue;

                long numero = long.Parse(retorno.documentoTransporte);

                DateTime data;
                if (!DateTime.TryParseExact(retorno.data, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out data))
                    data = DateTime.Now;

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(retorno.cnpjTranspEmit));

                if (empresa == null)
                {
                    Servicos.Log.TratarErro("Consulta da DT " + retorno.documentoTransporte + ": transportador não está cadastrado (" + retorno.cnpjTranspEmit + ").");
                    continue;
                }

                Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura = repDocumentoTransporte.BuscarPorNumero(empresa.Codigo, numero, false);

                if (dtNatura != null)
                {
                    if (dtNatura.Cargas.Any(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada))
                        continue;

                    repNotaFiscalDTNatura.DeletarPorDT(new List<int>() { dtNatura.Codigo });

                    //foreach (Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura notaFiscal in dtNatura.NotasFiscais)
                    //    repNotaFiscalDTNatura.Deletar(notaFiscal);
                }

                bool valorFreteNegativo = false;
                decimal valorFrete = (from obj in retorno.nfe select string.IsNullOrWhiteSpace(obj.informacoesTransporte.valorFrete) ? 0m : decimal.Parse(obj.informacoesTransporte.valorFrete, cultura)).Sum();

                if (valorFrete == 0)
                    valorFreteNegativo = true;

                if (autorizaValorZerado || !valorFreteNegativo)
                {
                    if (valorFreteNegativo)
                        Servicos.Log.TratarErro("DT Natura " + numero.ToString() + " com valor de frete zerado.");

                    string erroDT = string.Empty;

                    try
                    {
                        unidadeDeTrabalho.Start();

                        if (dtNatura == null)
                        {
                            dtNatura = new Dominio.Entidades.Embarcador.Integracao.DTNatura();
                            dtNatura.Empresa = empresa;
                            dtNatura.Numero = numero;
                            dtNatura.Integracoes = new List<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura>();
                        }

                        dtNatura.Data = data;
                        dtNatura.ValorFrete = valorFrete;
                        dtNatura.Integracoes.Add(integracaoNatura);
                        dtNatura.Status = true;

                        if (dtNatura.Codigo > 0)
                            repDocumentoTransporte.Atualizar(dtNatura);
                        else
                            repDocumentoTransporte.Inserir(dtNatura);

                        unidadeDeTrabalho.CommitChanges();

                        unidadeDeTrabalho.Start();

                        for (var i = 0; i < retorno.nfe.Count(); i++)
                        {
                            if (string.IsNullOrWhiteSpace(retorno.nfe[i].xmlNFe))
                                erroDT += "XML NF-e inexistente no DT. ";
                            if (string.IsNullOrWhiteSpace(retorno.nfe[i].chaveNFe))
                                erroDT += "Chave NF-e inexistente no DT. ";
                            if (retorno.nfe[i].informacoesPedido.emitente == null)
                                erroDT += "Emitente inexistente no DT. ";
                            if (retorno.nfe[i].informacoesPedido.destinatario == null)
                                erroDT += "Destinatário inexistente no DT. ";

                            if (!string.IsNullOrWhiteSpace(retorno.nfe[i].xmlNFe)) //POSSUI XML DE NF-e
                                SalvarNotaFiscal(dtNatura, retorno.nfe[i], unidadeDeTrabalho);
                            else //NOTA EM CONTINGENCIA (não vem XML, só os dados principais)
                                SalvarNotaFiscalContingencia(dtNatura, retorno.nfe[i], unidadeDeTrabalho);
                        }

                        unidadeDeTrabalho.CommitChanges();
                    }
                    catch (Exception ex)
                    {
                        unidadeDeTrabalho.Rollback();

                        Servicos.Log.TratarErro("Erro ao salvar a DT Natura " + numero.ToString() + ": " + ex);

                        if (dtNatura != null && dtNatura.Codigo > 0)
                        {
                            dtNatura.Status = false;
                            dtNatura.Observacao = erroDT;
                            dtNatura.Integracoes.Add(integracaoNatura);

                            repDocumentoTransporte.Atualizar(dtNatura);
                        }
                    }
                }
                else
                {
                    if (dtNatura == null)
                    {
                        dtNatura = new Dominio.Entidades.Embarcador.Integracao.DTNatura();
                        dtNatura.Empresa = empresa;
                        dtNatura.Numero = numero;
                        dtNatura.Integracoes = new List<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura>();
                    }

                    dtNatura.Integracoes.Add(integracaoNatura);
                    dtNatura.Data = data;
                    dtNatura.Status = false;
                    dtNatura.Observacao = "DT possui notas com valor de frete zerado.";

                    if (dtNatura.Codigo > 0)
                        repDocumentoTransporte.Atualizar(dtNatura);
                    else
                        repDocumentoTransporte.Inserir(dtNatura);
                }

            }
        }

        public static void EnviarRetornoDT(Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote lote, List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualIntegracaoLote repNFSManualIntegracaoLote = new Repositorio.Embarcador.NFS.NFSManualIntegracaoLote(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.CodigoMatrizNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaNatura))
                throw new Exception("Os dados para a integração com a Natura não estão configurados.");

            if (ctesDoLote.First().LancamentoNFSManual.IntegracoesNatura.Count() <= 0) //não possui DT vinculada à carga, então não envia o retorno e seta as integrações como problema (é para a carga ficar com problema na etapa de integração)
            {
                lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                lote.ProblemaIntegracao = "Nenhum documento de transporte da natura vinculado à carga.";

                repNFSManualIntegracaoLote.Atualizar(lote);

                foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao in ctesDoLote)
                {
                    nfsManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    nfsManualCTeIntegracao.ProblemaIntegracao = "Nenhum documento de transporte da natura vinculado à carga.";

                    repNFSManualCTeIntegracao.Atualizar(nfsManualCTeIntegracao);
                }

                return;
            }

            bool enviouComSucesso = EnviarRetornoDocumentoTransporteNFSManual(out Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoEnvioCTes, configuracaoIntegracao, lote, ctesDoLote, unidadeDeTrabalho);

            AtualizarInformacoesLote(enviouComSucesso, lote, ctesDoLote, arquivoEnvioCTes, unidadeDeTrabalho);
        }

        public static void EnviarRetornoDT(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote repOcorrenciaCTeIntegracaoLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.CodigoMatrizNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaNatura))
                throw new Exception("Os dados para a integração com a Natura não estão configurados.");

            if (ctesDoLote.First().CargaCTe.CargaCTeComplementoInfo?.CargaOcorrencia.DTNatura == null && ctesDoLote.First().CargaCTe.Carga.IntegracoesNatura.Count() <= 0) //não possui DT vinculada à carga, então não envia o retorno e seta as integrações como problema (é para a carga ficar com problema na etapa de integração)
            {
                lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                lote.ProblemaIntegracao = "Nenhum documento de transporte da natura vinculado à carga.";

                repOcorrenciaCTeIntegracaoLote.Atualizar(lote);

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao in ctesDoLote)
                {
                    ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    ocorrenciaCTeIntegracao.ProblemaIntegracao = "Nenhum documento de transporte da natura vinculado à carga.";

                    repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
                }

                return;
            }


            bool enviouComSucesso = true;
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoEnvioCTes = null;

            if (ctesDoLote.Any(o => o.CargaCTe.CargaCTeComplementoInfo != null))
                enviouComSucesso = EnviarRetornoDocumentoTransporte(out arquivoEnvioCTes, configuracaoIntegracao, lote, ctesDoLote, unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoEnvioOcorrencias = null;

            if (enviouComSucesso)
                arquivoEnvioOcorrencias = EnviarOcorrenciasDocumentoTransporte(configuracaoIntegracao, lote, ctesDoLote, unidadeDeTrabalho);

            AtualizarInformacoesLote(enviouComSucesso, lote, ctesDoLote, arquivoEnvioCTes, arquivoEnvioOcorrencias, unidadeDeTrabalho);
        }

        public static void EnviarRetornoDT(CargaCTeIntegracaoLote lote, List<CargaCTeIntegracao> ctesDoLote, UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.TratarErro(lote.Codigo + " - " + DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " - Inicio EnviarRetornoDT.");

            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote repCargaCTeIntegracaoLote = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            bool loteInvalido = false;
            string mensagemLoteInvalido = string.Empty;

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.CodigoMatrizNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaNatura))
            {
                loteInvalido = true;
                mensagemLoteInvalido = "Os dados para a integração com a Natura não estão configurados.";
            }
            else if (ctesDoLote.First().CargaCTe.Carga.IntegracoesNatura.Count() <= 0) //não possui DT vinculada à carga, então não envia o retorno e seta as integrações como problema (é para a carga ficar com problema na etapa de integração)
            {
                loteInvalido = true;
                mensagemLoteInvalido = "Nenhum documento de transporte da natura vinculado à carga.";
            }

            if (loteInvalido)
            {
                lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                lote.ProblemaIntegracao = mensagemLoteInvalido;
                lote.DataEnvio = DateTime.Now;
                lote.NumeroTentativas++;

                repCargaCTeIntegracaoLote.Atualizar(lote);
                repCargaCTeIntegracao.SetarRetornoPorLote(lote.Codigo, lote.NumeroTentativas, lote.ProblemaIntegracao, lote.SituacaoIntegracao, lote.DataEnvio.Value);

                return;
            }

            bool enviouComSucesso = EnviarRetornoDocumentoTransporte(out CargaCTeIntegracaoArquivo arquivoEnvioCTes, configuracaoIntegracao, lote, ctesDoLote, unidadeDeTrabalho);

            CargaCTeIntegracaoArquivo arquivoEnvioOcorrencias = null;

            if (enviouComSucesso && configuracaoIntegracao.EnviarOcorrenciaNaturaAutomaticamente)
            {
                Servicos.Log.TratarErro(lote.Codigo + " - " + DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " - Inicio EnviarOcorrenciasDocumentoTransporte.");

                arquivoEnvioOcorrencias = EnviarOcorrenciasDocumentoTransporte(configuracaoIntegracao, lote, ctesDoLote, unidadeDeTrabalho);

                Servicos.Log.TratarErro(lote.Codigo + " - " + DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " - Fim EnviarOcorrenciasDocumentoTransporte.");
            }

            Servicos.Log.TratarErro(lote.Codigo + " - " + DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " - Inicio AtualizarInformacoesLote.");

            AtualizarInformacoesLote(enviouComSucesso, lote, ctesDoLote, arquivoEnvioCTes, arquivoEnvioOcorrencias, unidadeDeTrabalho);

            Servicos.Log.TratarErro(lote.Codigo + " - " + DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " - Fim AtualizarInformacoesLote.");

            Servicos.Log.TratarErro(lote.Codigo + " - " + DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " - Fim EnviarRetornoDT.");
        }

        #endregion

        #region Métodos Privados

        private static void AtualizarInformacoesLote(bool enviouComSucesso, Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote lote, List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> documentosDoLote, Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoEnvio, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualIntegracaoLote repNFSManualIntegracaoLote = new Repositorio.Embarcador.NFS.NFSManualIntegracaoLote(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

            if (!enviouComSucesso)
                situacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            lote.NumeroTentativas++;
            lote.SituacaoIntegracao = situacaoIntegracao;
            lote.DataRecebimento = DateTime.Now;
            lote.ProblemaIntegracao = arquivoEnvio.Mensagem;

            repNFSManualIntegracaoLote.Atualizar(lote);

            foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao in documentosDoLote)
            {
                nfsManualCTeIntegracao.DataIntegracao = DateTime.Now;
                nfsManualCTeIntegracao.NumeroTentativas++;
                nfsManualCTeIntegracao.SituacaoIntegracao = situacaoIntegracao;
                nfsManualCTeIntegracao.ProblemaIntegracao = arquivoEnvio.Mensagem;
                nfsManualCTeIntegracao.ArquivosTransacao.Add(arquivoEnvio);
                repNFSManualCTeIntegracao.Atualizar(nfsManualCTeIntegracao);
            }
        }

        private static void AtualizarInformacoesLote(bool enviouComSucesso, CargaCTeIntegracaoLote lote, List<CargaCTeIntegracao> ctesDoLote, CargaCTeIntegracaoArquivo arquivoEnvioCTes, CargaCTeIntegracaoArquivo arquivoEnvioOcorrencias, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote repCargaCTeIntegracaoLote = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

            if (!enviouComSucesso)
                situacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            lote.NumeroTentativas++;
            lote.SituacaoIntegracao = situacaoIntegracao;
            lote.DataRecebimento = DateTime.Now;
            lote.ProblemaIntegracao = arquivoEnvioCTes?.Mensagem ?? (!enviouComSucesso ? "Ocorreu um erro ao efetuar a integração" : "");

            repCargaCTeIntegracaoLote.Atualizar(lote);
            repCargaCTeIntegracao.SetarRetornoPorLote(lote.Codigo, lote.NumeroTentativas, lote.ProblemaIntegracao, lote.SituacaoIntegracao, lote.DataRecebimento.Value);

            if (arquivoEnvioCTes != null)
                repCargaCTeIntegracao.SetarArquivoIntegracaoPorLote(lote.Codigo, arquivoEnvioCTes.Codigo);

            if (arquivoEnvioOcorrencias != null)
                repCargaCTeIntegracao.SetarArquivoIntegracaoPorLote(lote.Codigo, arquivoEnvioOcorrencias.Codigo);
        }

        private static void AtualizarInformacoesLote(bool enviouComSucesso, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ctesDoLote, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoEnvioCTes, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoEnvioOcorrencias, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote repOcorrenciaCTeIntegracaoLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

            if (!enviouComSucesso)
                situacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            lote.NumeroTentativas++;
            lote.SituacaoIntegracao = situacaoIntegracao;
            lote.DataRecebimento = DateTime.Now;
            lote.ProblemaIntegracao = arquivoEnvioCTes?.Mensagem ?? string.Empty;

            repOcorrenciaCTeIntegracaoLote.Atualizar(lote);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao in ctesDoLote)
            {
                ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;
                ocorrenciaCTeIntegracao.NumeroTentativas++;
                ocorrenciaCTeIntegracao.SituacaoIntegracao = situacaoIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = lote.ProblemaIntegracao;

                if (arquivoEnvioCTes != null)
                    ocorrenciaCTeIntegracao.ArquivosTransacao.Add(arquivoEnvioCTes);

                if (arquivoEnvioOcorrencias != null)
                    ocorrenciaCTeIntegracao.ArquivosTransacao.Add(arquivoEnvioOcorrencias);

                repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
            }
        }

        private static bool EnviarRetornoDocumentoTransporteNFSManual(out Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote lote, List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoNatura cargaIntegracaoNatura = lote.IntegracaoNatura;

            if (cargaIntegracaoNatura == null)
            {
                arquivoIntegracao = null;
                return false;
            }

            string mensagemRetorno = string.Empty;

            bool enviouComSucesso = EnviarRetornoDocumentoTransporte(out mensagemRetorno, out Servicos.Models.Integracao.InspectorBehavior inspector, configuracaoIntegracao, cargaIntegracaoNatura.DocumentoTransporte, ctesDoLote.Select(o => o.LancamentoNFSManual.CTe).ToList(), unidadeDeTrabalho);

            Repositorio.Embarcador.NFS.NFSManualIntegracaoArquivo repNFSManualIntegracaoArquivo = new Repositorio.Embarcador.NFS.NFSManualIntegracaoArquivo(unidadeDeTrabalho);

            arquivoIntegracao = new Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo();

            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeDeTrabalho);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeDeTrabalho);
            arquivoIntegracao.Data = DateTime.Now;
            arquivoIntegracao.Mensagem = mensagemRetorno;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repNFSManualIntegracaoArquivo.Inserir(arquivoIntegracao);

            return enviouComSucesso;
        }

        private static bool EnviarRetornoDocumentoTransporte(out CargaCTeIntegracaoArquivo arquivoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, CargaCTeIntegracaoLote lote, List<CargaCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura cargaIntegracaoNatura = lote.IntegracaoNatura;

            if (cargaIntegracaoNatura == null)
            {
                arquivoIntegracao = null;
                return false;
            }

            bool enviouComSucesso = false;

            try
            {
                string mensagemRetorno = string.Empty;

                enviouComSucesso = EnviarRetornoDocumentoTransporte(out mensagemRetorno, out Servicos.Models.Integracao.InspectorBehavior inspector, configuracaoIntegracao, cargaIntegracaoNatura.DocumentoTransporte, ctesDoLote.Select(o => o.CargaCTe.CTe).ToList(), unidadeDeTrabalho);

                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeDeTrabalho);

                arquivoIntegracao = new CargaCTeIntegracaoArquivo();

                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeDeTrabalho);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeDeTrabalho);
                arquivoIntegracao.Data = DateTime.Now;
                arquivoIntegracao.Mensagem = Utilidades.String.Left(mensagemRetorno, 500);
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                arquivoIntegracao = null;
                enviouComSucesso = false;
            }

            return enviouComSucesso;
        }

        private static bool EnviarRetornoDocumentoTransporte(out Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura cargaIntegracaoNatura = lote.IntegracaoNatura;

            Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura = ctesDoLote.First().CargaCTe.CargaCTeComplementoInfo.CargaOcorrencia.DTNatura;

            if (dtNatura == null && cargaIntegracaoNatura != null)
                dtNatura = cargaIntegracaoNatura.DocumentoTransporte;

            if (dtNatura == null)
            {
                arquivoIntegracao = null;
                return false;
            }

            bool enviouComSucesso = false;

            try
            {
                string mensagemRetorno = string.Empty;

                enviouComSucesso = EnviarRetornoDocumentoTransporte(out mensagemRetorno, out Servicos.Models.Integracao.InspectorBehavior inspector, configuracaoIntegracao, dtNatura, ctesDoLote.Select(o => o.CargaCTe.CTe).ToList(), unidadeDeTrabalho);

                Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(unidadeDeTrabalho);

                arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();

                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeDeTrabalho);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeDeTrabalho);
                arquivoIntegracao.Data = DateTime.Now;
                arquivoIntegracao.Mensagem = mensagemRetorno;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                arquivoIntegracao = null;
                enviouComSucesso = false;
            }

            return enviouComSucesso;
        }

        private static CargaCTeIntegracaoArquivo EnviarOcorrenciasDocumentoTransporte(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, CargaCTeIntegracaoLote lote, List<CargaCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura cargaIntegracaoNatura = lote.IntegracaoNatura;

            if (cargaIntegracaoNatura == null)
                return null;

            CargaCTeIntegracaoArquivo arquivoIntegracao = null;

            if (cargaIntegracaoNatura.DocumentoTransporte.GeradoPorNOTFIS)
            {
                if (EnviarEDIOcorrenciaNatura(out string mensagem, out string arquivo, configuracaoIntegracao, lote, ctesDoLote, unidadeDeTrabalho))
                    mensagem = "Envio das ocorrências realizado com sucesso.";

                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeDeTrabalho);

                arquivoIntegracao = new CargaCTeIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivo, "xml", unidadeDeTrabalho),
                    ArquivoResposta = null,
                    Data = DateTime.Now,
                    Mensagem = mensagem,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
                };

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            }
            else
            {
                Servicos.Models.Integracao.InspectorBehavior inspector = EnviarOcorrenciasDocumentoTransporte(configuracaoIntegracao, cargaIntegracaoNatura.DocumentoTransporte, ctesDoLote.Select(o => o.CargaCTe.CTe).ToList(), unidadeDeTrabalho, null);

                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeDeTrabalho);

                arquivoIntegracao = new CargaCTeIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeDeTrabalho),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeDeTrabalho),
                    Data = DateTime.Now,
                    Mensagem = "Envio das ocorrências realizado com sucesso.",
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
                };

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            }

            return arquivoIntegracao;
        }

        private static Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo EnviarOcorrenciasDocumentoTransporte(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura cargaIntegracaoNatura = lote.IntegracaoNatura;

            Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura = ctesDoLote.First().CargaCTe.CargaCTeComplementoInfo?.CargaOcorrencia.DTNatura;

            if (dtNatura == null && cargaIntegracaoNatura != null)
                dtNatura = cargaIntegracaoNatura.DocumentoTransporte;

            Servicos.Models.Integracao.InspectorBehavior inspector = EnviarOcorrenciasDocumentoTransporte(configuracaoIntegracao, dtNatura, ctesDoLote.Select(o => o.CargaCTe.CTe).ToList(), unidadeDeTrabalho, ctesDoLote.Select(o => o.CargaOcorrencia).FirstOrDefault());

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();

            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeDeTrabalho);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeDeTrabalho);
            arquivoIntegracao.Data = DateTime.Now;
            arquivoIntegracao.Mensagem = "Envio das ocorrências realizado com sucesso.";
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        private static Servicos.Models.Integracao.InspectorBehavior EnviarOcorrenciasDocumentoTransporte(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Integracao.DTNatura documentoTransporte, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia)
        {
            Repositorio.OcorrenciaDeCTe repOcorrencia = new Repositorio.OcorrenciaDeCTe(unidadeDeTrabalho);

            List<Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDadosOcorrencias> ocorrencias = new List<ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDadosOcorrencias>();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                Dominio.Entidades.OcorrenciaDeCTe ocorrencia = repOcorrencia.BuscarUltimaDoCTe(cte.Codigo);

                string codigoProceda = "01",
                       observacao = string.Empty;
                DateTime dataOcorrencia = DateTime.Now;

                if (cargaOcorrencia != null && cargaOcorrencia.TipoOcorrencia != null && !string.IsNullOrEmpty(cargaOcorrencia.TipoOcorrencia.CodigoProceda))
                    codigoProceda = cargaOcorrencia.TipoOcorrencia.CodigoProceda;
                else if (!string.IsNullOrEmpty(ocorrencia?.Ocorrencia?.CodigoProceda))
                    codigoProceda = ocorrencia?.Ocorrencia?.CodigoProceda;

                if (cargaOcorrencia != null)
                {
                    dataOcorrencia = cargaOcorrencia.DataOcorrencia;
                    observacao = cargaOcorrencia.Observacao;
                }
                else if (ocorrencia != null)
                {
                    dataOcorrencia = ocorrencia.DataDaOcorrencia;
                    observacao = ocorrencia.Observacao;
                }

                foreach (Dominio.Entidades.DocumentosCTE documento in cte.Documentos)
                {
                    ocorrencias.Add(new Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDadosOcorrencias()
                    {
                        chaveNFe = documento.ChaveNFE,
                        codigoOcorrencia = codigoProceda,
                        dataOcorrencia = dataOcorrencia.ToString("yyyy-MM-dd"),
                        horaOcorrencia = dataOcorrencia.ToString("HH:mm:ss"),
                        textoOcorrencia = observacao
                    });
                }
            }

            Servicos.ServicoNatura.ProcessaOcorrencias.SI_ProcessaOcorrenciasAsync_OBClient svcOcorrencia = ObterClientNatura<ServicoNatura.ProcessaOcorrencias.SI_ProcessaOcorrenciasAsync_OBClient, ServicoNatura.ProcessaOcorrencias.SI_ProcessaOcorrenciasAsync_OB>(configuracaoIntegracao.UsuarioNatura, configuracaoIntegracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_SI_ProcessaOcorrencias, unidadeDeTrabalho, out Servicos.Models.Integracao.InspectorBehavior inspector);

            svcOcorrencia.SI_ProcessaOcorrenciasAsync_OBAsync(new Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrencias()
            {
                dados = new Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDados()
                {
                    codTranspMatriz = configuracaoIntegracao.CodigoMatrizNatura,
                    documentoTransporte = documentoTransporte.Numero.ToString(),
                    ocorrencias = ocorrencias.ToArray()
                }
            });

            return inspector;
        }

        public static bool GerarDT(out string erro, out List<Dominio.Entidades.Embarcador.Integracao.DTNatura> dtsNatura, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Empresa empresa, Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis, UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, string stringConexao, string stringConexaoAdmin, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Log.TratarErro("iniciou GerarDT carga: " + (carga != null ? carga.CodigoCargaEmbarcador : " sem CARGA "));

            Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDT = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(unitOfWork);
            Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDocumentoTransporteNatura = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoNatura repCargaIntegracaoNatura = new Repositorio.Embarcador.Cargas.CargaIntegracaoNatura(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Servicos.Cliente svcCliente = new Cliente(stringConexao);

            dtsNatura = new List<Dominio.Entidades.Embarcador.Integracao.DTNatura>();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (empresa == null)
            {
                empresa = repEmpresa.BuscarEmpresaEmissoraPorUf(notfis.CabecalhoDocumento.Embarcadores[0].Pessoa.Endereco.Cidade.SiglaUF);

                if (empresa == null)
                    empresa = repEmpresa.BuscarPrincipalEmissoraTMS();

                if (empresa == null)
                {
                    erro = "É necessário selecionar a empresa/filial para importar este NOTFIS.";
                    return false;
                }
            }

            List<(string Cidade, string Estado)> localidades = notfis.CabecalhoDocumento.Embarcadores.SelectMany(embarcador => embarcador.Destinatarios.GroupBy(destinatario => (Cidade: Utilidades.String.RemoveDiacritics(destinatario?.Pessoa?.Endereco?.Cidade?.Descricao), Estado: destinatario?.Pessoa?.Endereco?.Cidade?.SiglaUF)).Select(grupo => grupo.Key)).ToList();
            List<string> localidadesInexistentes = new List<string>();

            foreach ((string Cidade, string Estado) localidade in localidades)
            {
                if (!repLocalidade.VerificarSeExisteLocalidade(localidade.Cidade, localidade.Estado))
                    localidadesInexistentes.Add($"{localidade.Cidade}/{localidade.Estado}");
            }

            if (localidadesInexistentes.Count > 0)
            {
                erro = $"Localidades não encontradas: {string.Join(", ", localidadesInexistentes)}.";
                return false;
            }

            string numeroDT = notfis.CabecalhoDocumento.Embarcadores.First().Destinatarios.First().NotasFiscais.First().NFe.NumeroRomaneio;

            long.TryParse(Utilidades.String.OnlyNumbers(numeroDT), out long numeroDTConvertido);

            if (numeroDTConvertido <= 0L)
            {
                erro = "Número do DT inválido no arquivo.";
                return false;
            }

            dtsNatura = repDocumentoTransporteNatura.BuscarTodosPorNumero(numeroDTConvertido, true);

            if (dtsNatura.Count > 0)
            {
                int codigoCarga = carga?.Codigo ?? 0;

                IEnumerable<int> codigosDTsNatura = dtsNatura.Select(o => o.Codigo);

                List<string> numerosCargas = repCargaIntegracaoNatura.BuscarDTsEmCarga(carga?.Codigo ?? 0, codigosDTsNatura);

                if (numerosCargas.Count > 0)
                {
                    erro = $"O DT do arquivo já está vinculado à(s) carga(s) {string.Join(", ", numerosCargas)}.";
                    return false;
                }

                repNotaFiscalDT.DeletarPorDT(codigosDTsNatura);
            }

            foreach (Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura in dtsNatura)
            {
                dtNatura.Data = DateTime.Now;
                dtNatura.Empresa = empresa;
                dtNatura.Observacao = "Gerado à partir de NOTFIS.";
                dtNatura.Status = true;
                dtNatura.ValorFrete = 0m;
                dtNatura.GeradoPorNOTFIS = true;

                repDocumentoTransporteNatura.Atualizar(dtNatura);
            }

            int numeroNotasFiscais = 0;

            foreach (Dominio.ObjetosDeValor.EDI.Notfis.Embarcador embarcador in notfis.CabecalhoDocumento.Embarcadores)
            {
                foreach (Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatario in embarcador.Destinatarios)
                {
                    foreach (Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaFiscal in destinatario.NotasFiscais)
                    {
                        unitOfWork.Start();

                        Servicos.Log.TratarErro("iniciou nova nota " + (notaFiscal.NFe != null ? notaFiscal.NFe.Numero.ToString() : " sem NFE "));

                        //caso o erro é de time out ou deadlock, vamos tentar outras vezes..
                        int tempo = 1000;
                        int quantidadeTentativas = 1;
                        while (true)
                        {
                            try
                            {
                                int NumeroNotaFiscal = 0;
                                if (notaFiscal.NFe != null)
                                    NumeroNotaFiscal = notaFiscal.NFe.Numero;

                                Servicos.Log.TratarErro("entrou no while " + quantidadeTentativas);

                                if (!unitOfWorkAdmin.IsOpenSession())
                                {
                                    unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(stringConexaoAdmin);
                                }

                                double cpfCnpjRecebedor = notaFiscal.ResponsavelFrete?.Pessoa?.CPFCNPJ?.ToDouble() ?? 0D;
                                Dominio.Entidades.Cliente recebedor = cpfCnpjRecebedor > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedor) : null;
                                if (recebedor == null && notaFiscal.ResponsavelFrete != null)
                                {
                                    notaFiscal.NFe.Recebedor = svcCliente.SetarDadosPessoa(notaFiscal.ResponsavelFrete.Pessoa, unitOfWorkAdmin, unitOfWork, true);
                                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoVerificacaoRecebedor = svcCliente.ConverterObjetoValorPessoa(notaFiscal.ResponsavelFrete.Pessoa, "recebedor", unitOfWork, 0, true, true);
                                    if (!retornoVerificacaoRecebedor.Status)
                                    {
                                        erro = retornoVerificacaoRecebedor.Mensagem;
                                        return false;
                                    }
                                    recebedor = retornoVerificacaoRecebedor.cliente;
                                }
                                Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura = dtsNatura.Where(o => o.Recebedor?.CPF_CNPJ == recebedor?.CPF_CNPJ).FirstOrDefault();

                                if (dtNatura == null)
                                {
                                    dtNatura = GerarDTNaturaNOTFIS(numeroDTConvertido, empresa, recebedor, unitOfWork);
                                    dtsNatura.Add(dtNatura);
                                }

                                if (!SalvarNotaFiscal(out erro, ref dtNatura, tipoServicoMultisoftware, embarcador, destinatario, notaFiscal, configuracaoIntegracao, unitOfWork, unitOfWorkAdmin, stringConexao, recebedor))
                                {

                                    erro = "Nota Fiscal " + NumeroNotaFiscal + ": " + erro;
                                    return false;
                                }

                                unitOfWork.CommitChanges();


                                numeroNotasFiscais += 1;
                                Servicos.Log.TratarErro("Salvou Nota " + NumeroNotaFiscal);
                                break;
                            }
                            catch (TimeoutException ti)
                            {
                                Servicos.Log.TratarErro("Time out, tentativa " + quantidadeTentativas + " ex: " + ti.Message);
                                unitOfWork.Rollback();
                                System.Threading.Thread.Sleep(tempo);
                                quantidadeTentativas++;

                                if (quantidadeTentativas == 10)
                                {
                                    erro = "ocorreu uma falha ao tentar inserir, tente novamente";
                                    return false;
                                }

                            }
                            catch (Exception ex)
                            {
                                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                                {
                                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;

                                    if (excecao.Number == 1205)
                                    {
                                        Servicos.Log.TratarErro("DeadLock2, tentativa " + quantidadeTentativas + " ex: " + excecao.Message);
                                        unitOfWork.Rollback();
                                        System.Threading.Thread.Sleep(tempo);
                                        quantidadeTentativas++;

                                        if (quantidadeTentativas == 10)
                                        {
                                            erro = "ocorreu uma falha ao tentar inserir, tente novamente";
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        Servicos.Log.TratarErro("Não identificado, tentativa " + quantidadeTentativas + " ex: " + excecao.Message);
                                        unitOfWork.Rollback();
                                        quantidadeTentativas++;

                                        if (quantidadeTentativas == 10)
                                        {
                                            erro = "ocorreu uma falha ao tentar inserir, tente novamente";
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    Servicos.Log.TratarErro("Erro genérico, tentativa " + quantidadeTentativas + " ex: " + ex.Message);
                                    erro = "ocorreu uma falha ao tentar inserir, tente novamente";
                                    unitOfWork.Rollback();
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            foreach (Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura in dtsNatura)
            {
                //BUCAR A DT, E BUSCAR O SOMATORIO DO VALOR DE FRETE DAS NOTAS DA DT
                decimal valorFrete = repNotaFiscalDT.BuscarValorFretePorDT(dtNatura.Codigo);
                dtNatura.ValorFrete = valorFrete;

                repDocumentoTransporteNatura.Atualizar(dtNatura);

                Servicos.Auditoria.Auditoria.Auditar(auditado, dtNatura, null, "Gerou o DT à partir da importação de um NOTFIS.", unitOfWork);
            }

            unitOfWork.CommitChanges();

            Servicos.Log.TratarErro("FINALIZOU GerarDT carga: " + (carga != null ? carga.CodigoCargaEmbarcador : " sem CARGA " + " total: " + dtsNatura.Count + " Notas: " + numeroNotasFiscais));

            erro = string.Empty;
            return true;
        }

        private static Dominio.Entidades.Embarcador.Integracao.DTNatura GerarDTNaturaNOTFIS(long numeroDT, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente recebedor, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDTNatura = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura = new Dominio.Entidades.Embarcador.Integracao.DTNatura()
            {
                Numero = numeroDT,
                Data = DateTime.Now,
                Empresa = empresa,
                Observacao = "Gerado à partir de NOTFIS.",
                Status = true,
                ValorFrete = 0m,
                GeradoPorNOTFIS = true,
                Recebedor = recebedor
            };

            repDTNatura.Inserir(dtNatura);

            return dtNatura;
        }

        private static bool EnviarRetornoDocumentoTransporte(out string mensagemRetorno, out Servicos.Models.Integracao.InspectorBehavior inspector, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Integracao.DTNatura documentoTransporte, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            mensagemRetorno = string.Empty;

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Servicos.CTe serCTe = new Servicos.CTe(unidadeDeTrabalho);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            List<ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal> documentosEnvio = new List<ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal>();

            string[] descricaoComponentesNaoSomar = new string[]
                {
                    "frete valor",
                    "valor frete",
                    "icms",
                    "iss",
                    "imposto",
                    "impostos"
                };

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                List<Dominio.Entidades.DocumentosCTE> documentos = null;

                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico ctePai = repCTe.BuscarPorChave(cte.ChaveCTESubComp);
                    documentos = ctePai.Documentos.ToList();
                }
                else
                    documentos = cte.Documentos.ToList();

                byte[] xml = cte.Status == "A" ? serCTe.ObterXMLAutorizacao(cte, unidadeDeTrabalho) : serCTe.ObterXMLCancelamento(cte, unidadeDeTrabalho);
                string stringXMLCTe = System.Text.Encoding.Default.GetString(xml);

                XmlDocument xDoc = new XmlDocument();

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                {
                    documentosEnvio.Add(new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal()
                    {
                        dataEmissao = cte.DataEmissao.Value,
                        destino = string.Format("{0:0000000}", cte.LocalidadeTerminoPrestacao.CodigoIBGE),
                        modelo = "07",
                        numero = cte.Numero.ToString(),
                        serie = cte.Serie.Numero.ToString(),
                        origem = string.Format("{0:0000000}", cte.LocalidadeInicioPrestacao.CodigoIBGE),
                        tipoDoc = cte.Status == "A" ? "N" : "A",
                        valorFrete = (cte.ValorFrete + cte.ComponentesPrestacao.Where(o => !descricaoComponentesNaoSomar.Contains(o.Nome.ToLower().Trim())).Sum(o => o.Valor)).ToString("0.00", cultura),
                        valorImpostos = cte.ValorISS.ToString("0.00", cultura),
                        nfse = new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscalNfse()
                        {
                            aliqCOFINS = "0.00",
                            aliqCSLL = "0.00",
                            aliqIR = "0.00",
                            aliqISS = cte.AliquotaISS.ToString("0.00", cultura),
                            aliqPIS = "0.00",
                            baseCOFINS = "0.00",
                            baseCSLL = "0.00",
                            baseIR = "0.00",
                            baseISS = cte.BaseCalculoISS.ToString("0.00", cultura),
                            basePIS = "0.00",
                            retencao = cte.ValorISSRetido.ToString("0.00", cultura),
                            valorCOFINS = "0.00",
                            valorCSLL = cte.ValorCSLL.ToString("0.00", cultura),
                            valorIR = cte.ValorIR.ToString("0.00", cultura),
                            valorISS = cte.ValorISS.ToString("0.00", cultura),
                            valorPIS = cte.ValorPIS.ToString("0.00", cultura)
                        },
                        notasTransportadas = (from obj in documentos
                                              select new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscalNotasTransportadas()
                                              {
                                                  chaveNFe = obj.ChaveNFE,
                                                  dataEmissao = obj.DataEmissao,
                                                  dataEmissaoSpecified = true
                                              }).ToArray()
                    });
                }
                else
                {
                    documentosEnvio.Add(new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal()
                    {
                        dataEmissao = cte.DataEmissao.Value,
                        destino = string.Format("{0:0000000}", cte.LocalidadeTerminoPrestacao.CodigoIBGE),
                        modelo = "57",
                        numero = cte.Numero.ToString(),
                        serie = cte.Serie.Numero.ToString(),
                        origem = string.Format("{0:0000000}", cte.LocalidadeInicioPrestacao.CodigoIBGE),
                        tipoDoc = cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao ? "A" :
                              cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento ? "C" :
                              cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto ? "S" :
                              cte.Status == "A" ? "N" : "A",
                        valorFrete = (cte.ValorFrete + cte.ComponentesPrestacao.Where(o => !descricaoComponentesNaoSomar.Contains(o.Nome.ToLower().Trim())).Sum(o => o.Valor)).ToString("0.00", cultura),
                        valorImpostos = cte.ValorICMS.ToString("0.00", cultura),
                        cte = new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscalCte()
                        {
                            chaveCTe = cte.Chave,
                            xmlCTe = xDoc.CreateCDataSection(stringXMLCTe.Replace("<![CDATA[", "").Replace("]]>", ""))
                        },
                        notasTransportadas = (from obj in documentos
                                              select new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscalNotasTransportadas()
                                              {
                                                  chaveNFe = obj.ChaveNFE,
                                                  dataEmissao = obj.DataEmissao,
                                                  dataEmissaoSpecified = true
                                              }).ToArray()
                    });
                }


            }

            inspector = new Models.Integracao.InspectorBehavior();

            if (documentosEnvio.Count > 0)
            {
                ServicoNaturaNovo.ProcessaCTeNFSe.SI_ProcessaCteNfse_SyncClient svcEnvioRetorno = ObterClientNatura<ServicoNaturaNovo.ProcessaCTeNFSe.SI_ProcessaCteNfse_SyncClient, ServicoNaturaNovo.ProcessaCTeNFSe.SI_ProcessaCteNfse_Sync>(configuracaoIntegracao.UsuarioNatura, configuracaoIntegracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_Novo_SI_ProcessaCteNfse, unidadeDeTrabalho, out inspector);

                try
                {
                    string[] codigosSucesso = new string[] { "100", "106", "109", "17" };

                    ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentResponseDados[] retorno = svcEnvioRetorno.SI_ProcessaCteNfse_Sync(new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequest()
                    {
                        dados = new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDados()
                        {
                            cnpjEmitente = ctes[0].Empresa.CNPJ,
                            cnpjTomador = ctes[0].TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? ctes[0].Remetente.CPF_CNPJ : ctes[0].Destinatario.CPF_CNPJ,
                            codTranspEmit = ctes[0].Empresa.Configuracao.CodigoFilialNatura,
                            codTranspMatriz = configuracaoIntegracao.CodigoMatrizNatura,
                            numeroTransporte = documentoTransporte.Numero.ToString(),
                            documentoFiscal = documentosEnvio.ToArray()
                        }
                    });

                    mensagemRetorno = string.Join(" / ", from obj in retorno select obj.number + " - " + obj.message);

                    if (retorno.All(o => o.number == "107" /*107 - CTes com erros no processo*/ || o.number == "41" /*41 - CTe: 011067572. Registro enviado já existente na base.*/))
                        return true;

                    if (retorno.Any(o => !codigosSucesso.Contains(o.number)))
                        return false;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    mensagemRetorno = ex.Message;
                    return false;
                }
            }

            return true;
        }

        private bool SalvarNotasFiscaisPedido(Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura, List<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura> notasFiscaisDTNatura, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unidadeDeTrabalho, out string mensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (cargaPedido.Pedido.Destinatario == null || dtNatura.GeradoPorNOTFIS)
            {
                cargaPedido.Pedido.Destinatario = notasFiscaisDTNatura[0].Destinatario;
                cargaPedido.Pedido.Destino = cargaPedido.Pedido.Destinatario.Localidade;
                cargaPedido.Destino = cargaPedido.Pedido.Destinatario.Localidade;

                repPedido.Atualizar(cargaPedido.Pedido);
                repCargaPedido.Atualizar(cargaPedido);
            }

            if (dtNatura.GeradoPorNOTFIS)
            {
                Dominio.Entidades.Cliente recebedor = notasFiscaisDTNatura.Where(o => o.Recebedor != null).Select(o => o.Recebedor).FirstOrDefault();

                if (recebedor != null)
                {
                    cargaPedido.Pedido.Recebedor = recebedor;
                    cargaPedido.Pedido.Destino = recebedor.Localidade;

                    cargaPedido.Recebedor = recebedor;
                    cargaPedido.Destino = recebedor.Localidade;
                    cargaPedido.TipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.ComRecebedor;

                    repPedido.Atualizar(cargaPedido.Pedido);
                    repCargaPedido.Atualizar(cargaPedido);
                }
            }

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura notaFiscal in notasFiscaisDTNatura)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = pedidoXMLNotasFiscais.Where(o => (!string.IsNullOrWhiteSpace(o.XMLNotaFiscal.Chave) && o.XMLNotaFiscal.Chave == notaFiscal.Chave) || (o.XMLNotaFiscal.Numero == notaFiscal.Numero && o.XMLNotaFiscal.Serie == notaFiscal.Serie.ToString())).Select(o => o.XMLNotaFiscal).FirstOrDefault();

                if (!dtNatura.GeradoPorNOTFIS)
                {
                    if (xmlNotaFiscal == null)
                        xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();

                    xmlNotaFiscal.Chave = Utilidades.String.OnlyNumbers(notaFiscal.Chave);
                    xmlNotaFiscal.TipoEmissao = Utilidades.Chave.ObterTipoEmissao(xmlNotaFiscal.Chave).ToString().ToEnum<TipoEmissaoNotaFiscal>();
                    xmlNotaFiscal.DataEmissao = notaFiscal.DataEmissao.HasValue ? notaFiscal.DataEmissao.Value : DateTime.Now;
                    xmlNotaFiscal.Destinatario = notaFiscal.Destinatario;
                    xmlNotaFiscal.Emitente = notaFiscal.Emitente;
                    xmlNotaFiscal.ModalidadeFrete = notaFiscal.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago : Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                    xmlNotaFiscal.Modelo = string.IsNullOrWhiteSpace(notaFiscal.Chave) ? "99" : "55";
                    xmlNotaFiscal.nfAtiva = true;
                    xmlNotaFiscal.Numero = notaFiscal.Numero;
                    xmlNotaFiscal.Peso = notaFiscal.Peso;
                    xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;
                    xmlNotaFiscal.Serie = notaFiscal.Serie.ToString();
                    xmlNotaFiscal.TipoDocumento = string.IsNullOrWhiteSpace(notaFiscal.Chave) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe;
                    xmlNotaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
                    xmlNotaFiscal.Valor = notaFiscal.Valor;
                    xmlNotaFiscal.ValorFreteEmbarcador = notaFiscal.ValorFrete;

                    if (!configuracaoIntegracao.UtilizarValorFreteTMSNatura)
                        xmlNotaFiscal.ValorFrete = notaFiscal.ValorFrete;

                    xmlNotaFiscal.ValorTotalProdutos = notaFiscal.Valor;
                    xmlNotaFiscal.Volumes = notaFiscal.Quantidade;
                    xmlNotaFiscal.XML = notaFiscal.XML ?? string.Empty;
                    xmlNotaFiscal.CNPJTranposrtador = (notaFiscal.DocumentoTransporte != null && notaFiscal.DocumentoTransporte.Empresa != null) ? notaFiscal.DocumentoTransporte.Empresa.CNPJ_SemFormato : "";
                    xmlNotaFiscal.Empresa = notaFiscal.DocumentoTransporte != null ? notaFiscal.DocumentoTransporte.Empresa : null;
                    xmlNotaFiscal.PlacaVeiculoNotaFiscal = string.Empty;
                    xmlNotaFiscal.Filial = cargaPedido.Carga.Filial;

                    if (xmlNotaFiscal.Codigo > 0)
                    {
                        repXMLNotaFiscal.Atualizar(xmlNotaFiscal);
                    }
                    else
                    {
                        xmlNotaFiscal.DataRecebimento = DateTime.Now;
                        repXMLNotaFiscal.Inserir(xmlNotaFiscal);

                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal();
                        pedidoXMLNotaFiscal.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;
                        pedidoXMLNotaFiscal.CargaPedido = cargaPedido;
                        pedidoXMLNotaFiscal.XMLNotaFiscal = xmlNotaFiscal;

                        repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);
                    }

                    serCargaNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, out bool msgAlertaObservacao, out bool notaFiscalEmOutraCarga);
                }
                else
                {
                    if (xmlNotaFiscal == null)
                    {
                        xmlNotaFiscal = repXMLNotaFiscal.BuscarPorChave(Utilidades.String.OnlyNumbers(notaFiscal.Chave));

                        if (xmlNotaFiscal == null)
                        {
                            xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();

                            xmlNotaFiscal.Chave = Utilidades.String.OnlyNumbers(notaFiscal.Chave);
                            xmlNotaFiscal.TipoEmissao = Utilidades.Chave.ObterTipoEmissao(xmlNotaFiscal.Chave).ToString().ToEnum<TipoEmissaoNotaFiscal>();
                            xmlNotaFiscal.DataEmissao = notaFiscal.DataEmissao.HasValue ? notaFiscal.DataEmissao.Value : DateTime.Now;
                            xmlNotaFiscal.Destinatario = notaFiscal.Destinatario;
                            xmlNotaFiscal.Emitente = notaFiscal.Emitente;
                            xmlNotaFiscal.ModalidadeFrete = notaFiscal.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago : Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                            xmlNotaFiscal.Modelo = string.IsNullOrWhiteSpace(notaFiscal.Chave) ? "99" : "55";
                            xmlNotaFiscal.nfAtiva = true;
                            xmlNotaFiscal.Numero = notaFiscal.Numero;
                            xmlNotaFiscal.Peso = notaFiscal.Peso;
                            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;
                            xmlNotaFiscal.Serie = notaFiscal.Serie.ToString();
                            xmlNotaFiscal.TipoDocumento = string.IsNullOrWhiteSpace(notaFiscal.Chave) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe;
                            xmlNotaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
                            xmlNotaFiscal.Valor = notaFiscal.Valor;
                            xmlNotaFiscal.ValorFreteEmbarcador = notaFiscal.ValorFrete;

                            if (!configuracaoIntegracao.UtilizarValorFreteTMSNatura)
                                xmlNotaFiscal.ValorFrete = notaFiscal.ValorFrete;

                            xmlNotaFiscal.ValorTotalProdutos = notaFiscal.Valor;
                            xmlNotaFiscal.Volumes = notaFiscal.Quantidade;
                            xmlNotaFiscal.XML = notaFiscal.XML ?? string.Empty;
                            xmlNotaFiscal.CNPJTranposrtador = (notaFiscal.DocumentoTransporte != null && notaFiscal.DocumentoTransporte.Empresa != null) ? notaFiscal.DocumentoTransporte.Empresa.CNPJ_SemFormato : "";
                            xmlNotaFiscal.Empresa = notaFiscal.DocumentoTransporte != null ? notaFiscal.DocumentoTransporte.Empresa : null;
                            xmlNotaFiscal.PlacaVeiculoNotaFiscal = string.Empty;
                            xmlNotaFiscal.Filial = cargaPedido.Carga.Filial;
                            xmlNotaFiscal.DataRecebimento = DateTime.Now;

                            repXMLNotaFiscal.Inserir(xmlNotaFiscal);
                        }

                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal
                        {
                            CargaPedido = cargaPedido,
                            XMLNotaFiscal = xmlNotaFiscal,
                            TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda
                        };

                        repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);
                    }
                }
            }

            mensagem = string.Empty;
            return true;
        }

        public static T ObterClientNatura<T, TChannel>(string usuario, string senha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao tipoWebService, Repositorio.UnitOfWork unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspectorBehavior) where TChannel : class where T : System.ServiceModel.ClientBase<TChannel>, new()
        {
            T svcNatura = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<T, TChannel>(tipoWebService, out inspectorBehavior);

            svcNatura.ClientCredentials.UserName.UserName = usuario;
            svcNatura.ClientCredentials.UserName.Password = senha;

            return svcNatura;
        }

        private static bool SalvarNotaFiscal(out string erro, ref Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.EDI.Notfis.Embarcador embarcador, Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatario, Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaFiscal, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, string stringConexao, Dominio.Entidades.Cliente recebedor)
        {
            erro = string.Empty;

            long.TryParse(Utilidades.String.OnlyNumbers(notaFiscal.NFe.NumeroRomaneio), out long numeroDTNotaFiscal);

            if (numeroDTNotaFiscal != dtNatura.Numero)
            {
                erro = "Existe mais de um número de DT no arquivo (" + dtNatura.Numero.ToString() + ", " + numeroDTNotaFiscal.ToString() + ").";
                return false;
            }

            Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDT = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Servicos.Cliente svcCliente = new Cliente(stringConexao);
            Servicos.Embarcador.Pedido.NotaFiscal svcNotaFiscal = new Pedido.NotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura notaFiscalDT = new Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura();

            notaFiscalDT.Chave = !string.IsNullOrWhiteSpace(notaFiscal.NFe.Chave) ? notaFiscal.NFe.Chave.Trim() : string.Empty;
            notaFiscalDT.DataEmissao = DateTime.ParseExact(notaFiscal.NFe.DataEmissao, "ddMMyy", null, System.Globalization.DateTimeStyles.None);

            embarcador.Pessoa = svcCliente.SetarDadosPessoa(embarcador.Pessoa, unitOfWorkAdmin, unitOfWork, true);

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoVerificacaoEmitente = svcCliente.ConverterObjetoValorPessoa(embarcador.Pessoa, "emitente", unitOfWork, 0, true, true);

            if (!retornoVerificacaoEmitente.Status)
            {
                erro = retornoVerificacaoEmitente.Mensagem;
                return false;
            }

            notaFiscal.NFe.Emitente = embarcador.Pessoa;

            destinatario.Pessoa = svcCliente.SetarDadosPessoa(destinatario.Pessoa, unitOfWorkAdmin, unitOfWork, true);

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoVerificacaoDestinatario = svcCliente.ConverterObjetoValorPessoa(destinatario.Pessoa, "destinatário", unitOfWork, 0, true, true);

            if (!retornoVerificacaoDestinatario.Status)
            {
                erro = retornoVerificacaoDestinatario.Mensagem;
                return false;
            }

            notaFiscal.NFe.Destinatario = destinatario.Pessoa;

            if (notaFiscal.Recebedor != null)
            {
                if (!string.IsNullOrWhiteSpace(notaFiscal.Recebedor.NumeroPedido))
                    notaFiscal.NFe.NumeroPedido = notaFiscal.Recebedor.NumeroPedido;
            }

            notaFiscalDT.Recebedor = recebedor;
            notaFiscal.NFe.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
            notaFiscal.NFe.DocumentoRecebidoViaNOTFIS = true;

            notaFiscalDT.Destinatario = retornoVerificacaoDestinatario.cliente;
            notaFiscalDT.DocumentoTransporte = dtNatura;

            notaFiscalDT.Emitente = retornoVerificacaoEmitente.cliente;

            notaFiscalDT.Numero = notaFiscal.NFe.Numero;
            notaFiscalDT.Peso = notaFiscal.NFe.PesoBruto;
            notaFiscalDT.NumeroPedido = notaFiscal.NFe.NumeroPedido;

            if (notaFiscalDT.Peso <= 0m)
                notaFiscalDT.Peso = notaFiscal.NFe.PesoLiquido;

            notaFiscalDT.Quantidade = (int)notaFiscal.NFe.VolumesTotal;
            notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            notaFiscalDT.Serie = int.Parse(notaFiscal.NFe.Serie);
            notaFiscalDT.Valor = notaFiscal.NFe.Valor;
            notaFiscalDT.ValorFrete = notaFiscal.NFe.ValorFreteLiquido;

            repNotaFiscalDT.Inserir(notaFiscalDT);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                if (notaFiscalDT.ValorFrete <= 0)
                {
                    erro = "Sem valor de Frete.";
                    return false;
                }
            }

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorChave(notaFiscal.NFe.Chave);
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorChaveAtiva(notaFiscal.NFe.Chave);

            if (pedidoXMLNotaFiscal != null)
            {
                erro = "Já existe uma NF-e com esta chave (" + xmlNotaFiscal.Chave + ") vinculada à um pedido (Pedido " + pedidoXMLNotaFiscal.CargaPedido.Pedido.Numero.ToString() + " - Carga " + pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador + ").";
                return false;
            }

            if (xmlNotaFiscal == null)
                xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();

            xmlNotaFiscal = svcNotaFiscal.PreencherParaXMLNotaFiscal(ref xmlNotaFiscal, notaFiscal.NFe, dtNatura.Empresa, null, ref erro);

            xmlNotaFiscal.ValorFreteEmbarcador = notaFiscalDT.ValorFrete;

            if (configuracaoIntegracao.UtilizarValorFreteTMSNatura)
                xmlNotaFiscal.ValorFrete = 0m;
            else
                xmlNotaFiscal.ValorFrete = notaFiscalDT.ValorFrete;

            if (!string.IsNullOrWhiteSpace(erro))
                return false;

            if (xmlNotaFiscal.Codigo > 0)
                repXMLNotaFiscal.Atualizar(xmlNotaFiscal);
            else
            {
                xmlNotaFiscal.DataRecebimento = DateTime.Now;

                repXMLNotaFiscal.Inserir(xmlNotaFiscal);
            }

            return true;
        }

        private void SalvarNotaFiscal(Dominio.Entidades.Embarcador.Integracao.DTNatura documentoTransporte, ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfe dados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            using (System.IO.Stream stream = Utilidades.String.ToStream(dados.xmlNFe))
            {
                object nota = MultiSoftware.NFe.Servicos.Leitura.Ler(stream);

                if (nota.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                    SalvarNotaFiscal(documentoTransporte, (MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)nota, dados, unidadeDeTrabalho);
                else if (nota.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc))
                    SalvarNotaFiscal(documentoTransporte, (MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)nota, dados, unidadeDeTrabalho);
                else
                    throw new Exception("Versão da NF-e não suportada. Implementar.");
            }
        }

        private void SalvarNotaFiscal(Dominio.Entidades.Embarcador.Integracao.DTNatura documentoTransporte, MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc notaFiscal, ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfe dados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDT = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(unidadeDeTrabalho);
            Servicos.NFe svcNFe = new Servicos.NFe(unidadeDeTrabalho);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura notaFiscalDT = new Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura();

            notaFiscalDT.Chave = !string.IsNullOrWhiteSpace(dados.chaveNFe) ? dados.chaveNFe.Trim() : string.Empty;
            notaFiscalDT.DataEmissao = DateTime.ParseExact(notaFiscal.NFe.infNFe.ide.dhEmi.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None);

            notaFiscalDT.Destinatario = svcNFe.ObterDestinatario(notaFiscal.NFe.infNFe.dest, documentoTransporte.Empresa.Codigo, unidadeDeTrabalho);

            notaFiscalDT.DocumentoTransporte = documentoTransporte;

            notaFiscalDT.Emitente = svcNFe.ObterEmitente(notaFiscal.NFe.infNFe.emit, documentoTransporte.Empresa.Codigo, unidadeDeTrabalho);

            notaFiscalDT.Numero = int.Parse(notaFiscal.NFe.infNFe.ide.nNF);
            notaFiscalDT.Peso = Math.Round(svcNFe.ObterPeso(notaFiscal.NFe.infNFe.transp, unidadeDeTrabalho), 4, MidpointRounding.ToEven);

            if (notaFiscalDT.Peso <= 0m)
                notaFiscalDT.Peso = Math.Round((decimal.Parse(dados.informacoesTransporte.pesoBruto.Trim(), cultura) / 1000), 4, MidpointRounding.ToEven);

            notaFiscalDT.Quantidade = svcNFe.ObterQuantidadeVolumes(notaFiscal.NFe.infNFe.transp);

            if (dados.informacoesTransporte.condFrete == "FOB")
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            else
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;

            notaFiscalDT.Serie = int.Parse(notaFiscal.NFe.infNFe.ide.serie);
            notaFiscalDT.Valor = decimal.Parse(notaFiscal.NFe.infNFe.total.ICMSTot.vNF, cultura);
            notaFiscalDT.ValorFrete = !string.IsNullOrWhiteSpace(dados.informacoesTransporte.valorFrete) ? decimal.Parse(dados.informacoesTransporte.valorFrete, cultura) : 0m;
            notaFiscalDT.XML = dados.xmlNFe;

            repNotaFiscalDT.Inserir(notaFiscalDT);
        }

        private void SalvarNotaFiscal(Dominio.Entidades.Embarcador.Integracao.DTNatura documentoTransporte, MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc notaFiscal, ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfe dados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDT = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(unidadeDeTrabalho);
            Servicos.NFe svcNFe = new Servicos.NFe(unidadeDeTrabalho);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura notaFiscalDT = new Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura();

            notaFiscalDT.Chave = !string.IsNullOrWhiteSpace(dados.chaveNFe) ? dados.chaveNFe.Trim() : string.Empty;
            notaFiscalDT.DataEmissao = DateTime.ParseExact(notaFiscal.NFe.infNFe.ide.dhEmi.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None);

            notaFiscalDT.Destinatario = svcNFe.ObterDestinatario(notaFiscal.NFe.infNFe.dest, documentoTransporte.Empresa.Codigo);

            notaFiscalDT.DocumentoTransporte = documentoTransporte;

            notaFiscalDT.Emitente = svcNFe.ObterEmitente(notaFiscal.NFe.infNFe.emit, documentoTransporte.Empresa.Codigo);

            notaFiscalDT.Numero = int.Parse(notaFiscal.NFe.infNFe.ide.nNF);
            notaFiscalDT.Peso = Math.Round(svcNFe.ObterPeso(notaFiscal.NFe.infNFe.transp), 4, MidpointRounding.ToEven);

            if (notaFiscalDT.Peso <= 0m)
                notaFiscalDT.Peso = Math.Round((decimal.Parse(dados.informacoesTransporte.pesoBruto.Trim(), cultura) / 1000), 4, MidpointRounding.ToEven);

            notaFiscalDT.Quantidade = svcNFe.ObterQuantidadeVolumes(notaFiscal.NFe.infNFe.transp);

            if (dados.informacoesTransporte.condFrete == "FOB")
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            else
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;

            notaFiscalDT.Serie = int.Parse(notaFiscal.NFe.infNFe.ide.serie);
            notaFiscalDT.Valor = decimal.Parse(notaFiscal.NFe.infNFe.total.ICMSTot.vNF, cultura);
            notaFiscalDT.ValorFrete = !string.IsNullOrWhiteSpace(dados.informacoesTransporte.valorFrete) ? decimal.Parse(dados.informacoesTransporte.valorFrete, cultura) : 0m;
            notaFiscalDT.XML = dados.xmlNFe;

            repNotaFiscalDT.Inserir(notaFiscalDT);
        }

        private void SalvarNotaFiscalContingencia(Dominio.Entidades.Embarcador.Integracao.DTNatura documentoTransporte, ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfe dados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDT = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(unidadeDeTrabalho);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura notaFiscalDT = new Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura();

            notaFiscalDT.Chave = !string.IsNullOrWhiteSpace(dados.chaveNFe) ? dados.chaveNFe.Trim() : string.Empty;
            if (dados.informacoesPedido.nfe.dataEmissao != "0000-00-00")
                notaFiscalDT.DataEmissao = DateTime.ParseExact(dados.informacoesPedido.nfe.dataEmissao, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None);
            else
                notaFiscalDT.DataEmissao = DateTime.Now;

            notaFiscalDT.Destinatario = ObterDestinatarioNotaFiscalContingencia(documentoTransporte.Empresa.Codigo, dados.informacoesPedido.destinatario, unidadeDeTrabalho);

            notaFiscalDT.Emitente = ObterEmitenteNotaFiscalContingencia(documentoTransporte.Empresa.Codigo, dados.informacoesPedido.emitente, unidadeDeTrabalho);

            notaFiscalDT.DocumentoTransporte = documentoTransporte;

            notaFiscalDT.Numero = int.Parse(dados.informacoesPedido.nfe.nNF);
            notaFiscalDT.Peso = Math.Round(decimal.Parse(dados.informacoesTransporte.pesoBruto, cultura), 4, MidpointRounding.ToEven);

            if (dados.informacoesTransporte.condFrete == "FOB")
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            else
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;

            notaFiscalDT.Quantidade = int.Parse(dados.informacoesTransporte.qtdVolumes);
            notaFiscalDT.Serie = int.Parse(dados.informacoesPedido.nfe.serie);
            notaFiscalDT.Valor = decimal.Parse(dados.informacoesPedido.nfe.icmsTot.vNF.Trim(), cultura);

            notaFiscalDT.ValorFrete = decimal.Parse(dados.informacoesTransporte.valorFrete, cultura);

            repNotaFiscalDT.Inserir(notaFiscalDT);
        }

        private Dominio.Entidades.Cliente ObterEmitenteNotaFiscalContingencia(int codigoEmpresa, ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfeInformacoesPedidoEmitente emitente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            double cpfCnpj;
            double.TryParse(Utilidades.String.OnlyNumbers(emitente.cnpj), out cpfCnpj);

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente != null)
                return cliente;

            cliente = new Dominio.Entidades.Cliente();
            cliente.Bairro = emitente.bairro.Length > 2 ? emitente.bairro : "Não Informado";
            cliente.CEP = emitente.cep;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.DataCadastro = DateTime.Now;
            cliente.Endereco = emitente.logradouro;
            cliente.IE_RG = emitente.inscricaoEstadual;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(emitente.codMunicipio));
            cliente.Nome = emitente.nomeEmitente;
            cliente.NomeFantasia = emitente.nomeFantasia;
            cliente.Numero = string.IsNullOrWhiteSpace(emitente.numero) ? "S/N" : emitente.numero;
            cliente.Telefone1 = string.IsNullOrWhiteSpace(emitente.telefone) || emitente.telefone.StartsWith("00") ? string.Empty : emitente.telefone;
            cliente.Tipo = "J";
            cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao);

            if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                if (grupoPessoas != null)
                {
                    cliente.GrupoPessoas = grupoPessoas;
                }
            }
            cliente.Ativo = true;
            repCliente.Inserir(cliente);

            return cliente;
        }

        private Dominio.Entidades.Cliente ObterDestinatarioNotaFiscalContingencia(int codigoEmpresa, ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfeInformacoesPedidoDestinatario destinatario, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            bool pf = true;

            double cpfCnpj;
            double.TryParse(Utilidades.String.OnlyNumbers(destinatario.cpf), out cpfCnpj);

            if (cpfCnpj <= 0)
            {
                double.TryParse(Utilidades.String.OnlyNumbers(destinatario.cnpj), out cpfCnpj);
                pf = false;
            }

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente != null)
                return cliente;

            cliente = new Dominio.Entidades.Cliente();
            cliente.Bairro = destinatario.bairro.Length > 2 ? destinatario.bairro : "Não Informado";
            cliente.CEP = destinatario.cep;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.DataCadastro = DateTime.Now;
            cliente.Endereco = destinatario.logradouro;
            cliente.IE_RG = destinatario.inscricaoEstadual;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(destinatario.codMunicipio));
            cliente.Nome = destinatario.nomeDestinatario;
            cliente.Numero = string.IsNullOrWhiteSpace(destinatario.numero) ? "S/N" : destinatario.numero;
            cliente.Telefone1 = string.IsNullOrWhiteSpace(destinatario.telefone) || destinatario.telefone.StartsWith("00") ? string.Empty : destinatario.telefone;
            cliente.Tipo = pf ? "F" : "J";
            cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao);

            if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                if (grupoPessoas != null)
                {
                    cliente.GrupoPessoas = grupoPessoas;
                }
            }
            cliente.Ativo = true;
            repCliente.Inserir(cliente);

            return cliente;
        }

        private static bool EnviarEDIOcorrenciaNatura(out string erro, out string arquivo, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (ctesDoLote == null || ctesDoLote.Count == 0)
            {
                erro = "Nenhum CT-e disponível para envio da ocorrência.";
                arquivo = null;
                return false;
            }

            Dominio.Entidades.Empresa empresa = ctesDoLote.First().CargaCTe.CTe.Empresa;

            if (string.IsNullOrEmpty(empresa.Configuracao.FTPNaturaHost) || string.IsNullOrEmpty(empresa.Configuracao.FTPNaturaPorta) || string.IsNullOrEmpty(empresa.Configuracao.FTPNaturaDiretorio))
            {
                erro = $"Endereço de FTP não definido para empresa/filial {empresa.RazaoSocial}.";
                arquivo = null;
                return false;
            }

            System.IO.MemoryStream memoStream = new System.IO.MemoryStream();
            System.Text.StringBuilder registroEDI = new System.Text.StringBuilder();

            registroEDI.Append("341")
                       .Append(empresa.CNPJ)
                       .Append(Utilidades.String.Left(empresa.RazaoSocial, 40).PadRight(40))
                       .AppendLine();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cte in ctesDoLote)
            {
                int tempoViagemMinutos = cte.CargaCTe.Carga.Rota?.TempoDeViagemEmMinutos ?? 0;

                DateTime? dataOcorrencia = cte.CargaCTe.CTe.DataEmissao?.AddMinutes(tempoViagemMinutos);

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in cte.CargaCTe.CTe.XMLNotaFiscais)
                {
                    registroEDI.Append("342");
                    registroEDI.Append(xmlNotaFiscal.Emitente.CPF_CNPJ_SemFormato.PadLeft(14));
                    registroEDI.Append("012");
                    registroEDI.Append(xmlNotaFiscal.Numero.ToString().PadLeft(9, '0'));
                    registroEDI.Append("80");
                    registroEDI.Append(dataOcorrencia?.ToString("ddMMyyyyHHmm"));
                    registroEDI.Append(new string(' ', 78));

                    registroEDI.AppendLine();
                }
            }

            arquivo = registroEDI.ToString();

            memoStream.Write(System.Text.Encoding.UTF8.GetBytes(arquivo), 0, arquivo.Length);

            memoStream.Position = 0;

            string nomeArquivo = "SAPOCO" + Utilidades.String.OnlyNumbers(empresa.Configuracao.CodigoFilialNatura) + DateTime.Now.ToString("ddMMyyHHmmssff") + ".txt";

            string host = empresa.Configuracao.FTPNaturaHost;
            string porta = empresa.Configuracao.FTPNaturaPorta;
            string diretorio = empresa.Configuracao.FTPNaturaDiretorio;
            string usuario = empresa.Configuracao.FTPNaturaUsuario;
            string senha = empresa.Configuracao.FTPNaturaSenha;

            bool passivo = empresa.Configuracao.FTPNaturaPassivo;
            bool sFTP = empresa.Configuracao.FTPNaturaSeguro;

            return Servicos.FTP.EnviarArquivo(memoStream, nomeArquivo, host, porta, diretorio, usuario, senha, passivo, false, out erro, sFTP);
        }

        #endregion
    }
}
