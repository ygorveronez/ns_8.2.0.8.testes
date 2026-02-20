using Dominio.Entidades.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using iTextSharp.text;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using Servicos.Servico.Electrolux.NOTFIS.Detalhe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Integracao.Electrolux
{
    public class IntegracaoElectroluxNOTFIS_Detalhe : IntegracaoElectroluxBase
    {

        #region Atributos Globais

        private readonly SI_DetailNotfis_EDI_Sync_OutboundRequest _request;
        private readonly SI_DetailNotfis_EDI_Sync_OutboundResponse _response;
        private readonly string _identificadorTransportador; 
        private readonly string _identificadorNotfis;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Usuario _usuario;

        private string _xmlResult = "";
        private XDocument _xDocumentResult;

        #endregion Atributos Globais

        #region CONSTRUTOR

        /// <summary>
        /// NOTFIS - Electrolux - Detalhes
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="identificadorTransportador">Identificação do transportador</param>
        /// <param name="dataInicial">Data Inicial</param>
        /// <param name="dataFinal">DataFinal</param>
        public IntegracaoElectroluxNOTFIS_Detalhe(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, string identificadorTransportador, string identificadorNotfis) : base (unitOfWork)
        {
            _usuario = usuario;
            _unitOfWork = unitOfWork;
            _request = new SI_DetailNotfis_EDI_Sync_OutboundRequest();
            _response = new SI_DetailNotfis_EDI_Sync_OutboundResponse();
            _identificadorTransportador = identificadorTransportador;
            _identificadorNotfis = identificadorNotfis;
            _xDocumentResult = new XDocument();
            _configuracaoIntegracaoElectroluxDominio = _configuracaoIntegracaoElectroluxRepositorio.BuscarPrimeiroRegistro();

        }

        public IntegracaoElectroluxNOTFIS_Detalhe(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork) : base(unitOfWork)
        {
            _usuario = usuario;
            _unitOfWork = unitOfWork;
            _request = new SI_DetailNotfis_EDI_Sync_OutboundRequest();
            _response = new SI_DetailNotfis_EDI_Sync_OutboundResponse();
            _xDocumentResult = new XDocument();
        }
        #endregion

        #region PÚBLICOS

        #endregion

        #region PRIVADOS

        public async Task ConsultarNOTFISAsync(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte documentoTransporte)
        {
            Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte repDocumentoTransporte = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte(_unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoElectroluxConsultaLog repIntegracaoElectroluxConsultaLog = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxConsultaLog(_unitOfWork);

            try
            {

                using (var httpClient = ObterClient(_configuracaoIntegracaoElectroluxDominio.UrlNotfisDetalhada))
                {
                    Models.Integracao.InspectorBehavior inspectorBehavior = new Models.Integracao.InspectorBehavior();

                    httpClient.Endpoint.EndpointBehaviors.Add(inspectorBehavior);

                    var soapEnvelope = GerarRequestEnvelope();

                    httpClient.ClientCredentials.UserName.UserName = _configuracaoIntegracaoElectroluxDominio.Usuario;
                    httpClient.ClientCredentials.UserName.Password = _configuracaoIntegracaoElectroluxDominio.Senha;

                    Servicos.Servico.Electrolux.NOTFIS.Detalhe.SI_DetailNotfis_EDI_Sync_OutboundResponse response = await httpClient.SI_DetailNotfis_EDI_Sync_OutboundAsync(soapEnvelope);

                    if (response.MT_ResultNotifisDetail_EDI != null)
                    {
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxConsultaLog integracao = new Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxConsultaLog();
                        integracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorBehavior.LastRequestXML, "xml", _unitOfWork);
                        integracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorBehavior.LastResponseXML, "xml", _unitOfWork);
                        integracao.DataConsulta = DateTime.Now;
                        integracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoElectrolux.Sucesso;
                        integracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoElectrolux.NotfisDetalhe;
                        integracao.Retorno = "Consulta de DT realizada com sucesso.";
                        integracao.RetornoIdentificadorDocumento = _identificadorNotfis.ToString();
                        integracao.Usuario = _usuario;
                        repIntegracaoElectroluxConsultaLog.Inserir(integracao);
                        documentoTransporte.Integracoes.Add(integracao);
                        repDocumentoTransporte.Atualizar(documentoTransporte);
                        
                        SalvarDT(empresa, response?.MT_ResultNotifisDetail_EDI,inspectorBehavior.LastRequestXML, inspectorBehavior.LastResponseXML);
                    }
                    else
                    {
                        throw new Exception("Consulta detalhada não retornou dados");
                    }

                }
            }
            catch (Exception ex)
            {
                documentoTransporte.Status = false;
                documentoTransporte.Observacao = ex.Message;
                repDocumentoTransporte.Atualizar(documentoTransporte);
                Log.TratarErro(ex);

            }
        }

        public bool VincularCargaAoDT(Dominio.Entidades.Usuario usuario, List<int> codigosDT, int codigoCargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string mensagem)
        {
            
            Servicos.Embarcador.Carga.Frete svcFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
            Servicos.Embarcador.Carga.CTe serCargaCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Servicos.Embarcador.Carga.RateioFormula serRateioFormula = new Carga.RateioFormula(unitOfWork);
            Servicos.Embarcador.Carga.Rota serRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte repDocumentoTransporte = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoElectrolux repCargaIntegracaoElectrolux = new Repositorio.Embarcador.Cargas.CargaIntegracaoElectrolux(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal repNotaFiscalDT = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);


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
                Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte documentoTransporte = repDocumentoTransporte.BuscarPorCodigo(codigoDT);
                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal> notasFiscaisDT = repNotaFiscalDT.BuscarPorDT(codigoDT);

                if (documentoTransporte == null)
                {
                    mensagem = "Documento de transporte da Electrolux não encontrado.";
                    return false;
                }

                if (documentoTransporte.Cargas.Where(obj => obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                                                 obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada).Any())
                {
                    mensagem = "O documento de transporte (" + documentoTransporte.NumeroNotfis.ToString() + ") da Electrolux já está vinculado à carga " + documentoTransporte.Cargas[0].Carga.CodigoCargaEmbarcador + ".";
                    return false;
                }

                if (notasFiscaisDT.Count <= 0)
                {
                    mensagem = "O documento de transporte (" + documentoTransporte.NumeroNotfis.ToString() + ") da Electrolux não possui notas fiscais vinculadas.";
                    return false;
                }

                if (documentoTransporte.GeradoPorNOTFIS)
                    geradoPorNOTFIS = true;

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoElectrolux cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoElectrolux
                {
                    Carga = carga,
                    CargaPedido = cargaPedido,
                    DocumentoTransporte = documentoTransporte,
                    Usuario = usuario
                };

                repCargaIntegracaoElectrolux.Inserir(cargaIntegracao);

                if (podeAlterarCarga)
                {
                    if (!SalvarNotasFiscaisPedido(documentoTransporte, notasFiscaisDT, cargaPedido, unitOfWork, out mensagem, tipoServicoMultisoftware))
                        return false;
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXML = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo); //(from obj in carga.Pedidos select repPedidoXMLNotaFiscal.BuscarPorCargaPedido(obj.Codigo)).SelectMany(o => o).ToList();

                    foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal notaFiscalDT in notasFiscaisDT)
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
                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoElectrolux> integracoesNatura = repCargaIntegracaoElectrolux.BuscarPorCargaPedido(cargaPedido.Codigo);

                decimal peso = repNotaFiscalDT.BuscarPesoPorDT(integracoesNatura.Select(o => o.DocumentoTransporte.Codigo)); 
                int volumes = repNotaFiscalDT.BuscarVolumesPorDT(integracoesNatura.Select(o => o.DocumentoTransporte.Codigo)); 

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

                serCargaPedido.AdicionarCargaPedidoQuantidades(cargaPedidoQuantidades, cargaPedido, unitOfWork);

                //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {peso}. ImportacaoDTNatura.VincularCargaAoDT", "PesoCargaPedido");
                cargaPedido.Peso = peso;

                repPedido.Atualizar(cargaPedido.Pedido);
                repCargaPedido.Atualizar(cargaPedido);

                Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = serRateioFormula.ObterFormulaDeRateio(carga, unitOfWork, cargaPedido);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos> tiposEmissaoCTeDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos>();
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes> tipoEmissaoCTeParticipantes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPed in cargaPedidos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumento = serCargaCTe.BuscarTipoEmissaoDocumentosCTe(cargaPed, tipoServicoMultisoftware, unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoParticipante = serCargaCTe.BuscarTipoEmissaoCTeParticipantes(cargaPed, tipoServicoMultisoftware, unitOfWork, false);

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

                serRota.DeletarPercursoDestinosCarga(carga, unitOfWork);

                serCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, cargaPedidos, unitOfWork, tipoServicoMultisoftware, configuracaoPedido);

                carga.ValorFreteEmbarcador = repCargaIntegracaoElectrolux.BuscarValorFretePorCargaPedido(cargaPedido.Codigo); //integracoesNatura.Sum(o => o.DocumentoTransporte.ValorFrete);
                carga.ValorFrete = carga.ValorFreteEmbarcador;
                carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                carga.PossuiPendencia = false;

                Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Embarcador", " Valor Informado = " + carga.ValorFrete.ToString("n2"), carga.ValorFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Embarcador", 0, carga.ValorFrete);
                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicaoFrete, unitOfWork, null);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    serRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, false, unitOfWork, tipoServicoMultisoftware);

                repCarga.Atualizar(carga);
            }
            
            mensagem = string.Empty;
            
            return true;
        }

        public bool GerarCargaPorDT(Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte documentoTransporte, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, out int codigoCarga, out string msgErro)
        {
            
            msgErro = "";
            codigoCarga = 0;

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal nota = documentoTransporte.NotasFiscais.FirstOrDefault();
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoPadrao = repTipoOperacao.BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracao();

            if (nota == null)
            {
                msgErro = "O documento de transporte (" + documentoTransporte.NumeroNotfis.ToString() + ") da Electrolux não possui notas fiscais vinculadas.";
                return false;
            }

            if(tipoOperacaoPadrao == null)
            {
                msgErro = "Nenhum tipo de operação padrão de integração definido";
                return false;
            }
            // Cria Pedido


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
                NumeroPedidoEmbarcador = documentoTransporte.NumeroNotfis.ToString(),
                Origem = nota.Emitente.Localidade,
                Destino = nota.Destinatario.Localidade,
                Empresa = documentoTransporte.Empresa,
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

        private bool SalvarNotasFiscaisPedido(Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte documentoTransporte, List<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal> notasFiscaisDT, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unidadeDeTrabalho, out string mensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (cargaPedido.Pedido.Destinatario == null || documentoTransporte.GeradoPorNOTFIS)
            {
                cargaPedido.Pedido.Destinatario = notasFiscaisDT[0].Destinatario;
                cargaPedido.Pedido.Destino = cargaPedido.Pedido.Destinatario.Localidade;
                cargaPedido.Destino = cargaPedido.Pedido.Destinatario.Localidade;

                repPedido.Atualizar(cargaPedido.Pedido);
                repCargaPedido.Atualizar(cargaPedido);
            }

            if (documentoTransporte.GeradoPorNOTFIS)
            {
                Dominio.Entidades.Cliente recebedor = notasFiscaisDT.Where(o => o.Recebedor != null).Select(o => o.Recebedor).FirstOrDefault();

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

            foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal notaFiscal in notasFiscaisDT)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = pedidoXMLNotasFiscais.Where(o => (!string.IsNullOrWhiteSpace(o.XMLNotaFiscal.Chave) && o.XMLNotaFiscal.Chave == notaFiscal.Chave) || (o.XMLNotaFiscal.Numero == notaFiscal.Numero && o.XMLNotaFiscal.Serie == notaFiscal.Serie.ToString())).Select(o => o.XMLNotaFiscal).FirstOrDefault();

                if (!documentoTransporte.GeradoPorNOTFIS)
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
        private DT_RequestNotfisDetail_SAP GerarRequestEnvelope() 
        {
            return new DT_RequestNotfisDetail_SAP() 
            { 
                identService = new DT_IdentService_SAP() 
                { 
                    identCarrier = _identificadorTransportador 
                }, 
                identNotfis = new DT_IdentNotfis() 
                { 
                    ident = _identificadorNotfis
                } 
            };

        }
        private static Servicos.Servico.Electrolux.NOTFIS.Detalhe.SI_DetailNotfis_EDI_Sync_OutboundClient ObterClient(string url)
        {
            var endpointAddress = new System.ServiceModel.EndpointAddress(url);
            var binding = new System.ServiceModel.BasicHttpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                ReceiveTimeout = TimeSpan.FromMinutes(5)
            };

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic; // Ajuste conforme necessário
            }

            return new Servicos.Servico.Electrolux.NOTFIS.Detalhe.SI_DetailNotfis_EDI_Sync_OutboundClient(binding, endpointAddress);
        }

        /// <summary>
        /// Monta um retorno em lista para o result
        /// </summary>
        /// <param name="xml">xml</param>
        /// 

        private void SalvarDT(Dominio.Entidades.Empresa empresa, Servicos.Servico.Electrolux.NOTFIS.Detalhe.DT_ReturnNotfisDetail_SAP retorno, string xmlRequest, string xmlResponse)
        {
            Repositorio.Embarcador.Integracao.IntegracaoElectroluxConsultaLog repIntegracaoElectroluxConsultaLog = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxConsultaLog(_unitOfWork);

            Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte repDocumentoTransporte = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte documentoTransporte = repDocumentoTransporte.BuscarPorNumero(empresa.Codigo, _identificadorNotfis, true);

            if (retorno.notfis?.registro000 == null)
                throw new Exception("Notfis não retornado na consulta detalhada");
            
            if (documentoTransporte != null)
            {
                if (documentoTransporte.Cargas != null && documentoTransporte.Cargas.Any(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada))
                    return;

                repDocumentoTransporte.DeletarPorDT(new List<int>() { documentoTransporte.Codigo });

                DateTime data = retorno.notfis.registro000.detail?.data ?? DateTime.Now;

                bool valorFreteNegativo = false;
                decimal valorFrete = retorno.notfis.registro000.registro500
                    .SelectMany(obj => obj.registro501)
                    .SelectMany(obj2 => obj2.registro503)
                    .SelectMany(obj3 => obj3.registro505)
                    .Sum(obj3 => obj3.registro507?.vTotFret ?? 0);


                if (valorFrete == 0)
                    valorFreteNegativo = true;

                if (!valorFreteNegativo)
                {
                    string erroDT = string.Empty;

                    documentoTransporte.GeradoPorNOTFIS = true;
                    documentoTransporte.Data = data;
                    documentoTransporte.ValorFrete = valorFrete;
                    documentoTransporte.Status = true;

                    if (documentoTransporte.Codigo > 0)
                        repDocumentoTransporte.Atualizar(documentoTransporte);
                    else
                        repDocumentoTransporte.Inserir(documentoTransporte);

                    foreach (var reg500 in retorno.notfis.registro000.registro500)
                    {
                        foreach (var reg501 in reg500.registro501)
                        {
                            foreach (var reg503 in reg501.registro503)
                            {
                                foreach (var reg505 in reg503.registro505)
                                {
                                    if (string.IsNullOrWhiteSpace(reg505.chaveAcessoNfe))
                                        erroDT += "Chave NF-e inexistente no DT. ";
                                    if (reg501.cnpjEmbarcad == null)
                                        erroDT += "Emitente inexistente no DT. ";
                                    if (reg503.cnpjCpfDest == null)
                                        erroDT += "Destinatário inexistente no DT. ";

                                    SalvarNotaFiscal(documentoTransporte, reg505, reg503, reg501, reg500);
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(erroDT))
                    {
                        documentoTransporte.Status = false;
                        documentoTransporte.Observacao = erroDT;
                        repDocumentoTransporte.Atualizar(documentoTransporte);
                    }
                    else
                    {
                        documentoTransporte.Status = true;
                        documentoTransporte.Observacao = "Documento de transporte integrado com sucesso";
                        repDocumentoTransporte.Atualizar(documentoTransporte);
                    }
                }
                else
                {
                    documentoTransporte.Data = data;
                    documentoTransporte.ValorFrete = valorFrete;
                    documentoTransporte.Status = false;
                    documentoTransporte.Observacao = "DT possui notas com valor de frete zerado.";

                    if (documentoTransporte.Codigo > 0)
                        repDocumentoTransporte.Atualizar(documentoTransporte);
                    else
                        repDocumentoTransporte.Inserir(documentoTransporte);
                }
            }           
        }
        private void SalvarNotaFiscal(Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte documentoTransporte, DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505 reg505, DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503 reg503, DT_NotfisDetail_SAPRegistro000Registro500Registro501 reg501, DT_NotfisDetail_SAPRegistro000Registro500 reg500)
        {

            Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal repNotaFiscalDT = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal(_unitOfWork);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal notaFiscalDT = new Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal();

            notaFiscalDT.Chave = !string.IsNullOrWhiteSpace(reg505.chaveAcessoNfe) ? reg505.chaveAcessoNfe.Trim() : string.Empty;
            notaFiscalDT.DataEmissao = reg505.dtEmissao;

            notaFiscalDT.Destinatario = this.ObterDestinatario(reg503, documentoTransporte.Empresa.Codigo);

            notaFiscalDT.DocumentoTransporte = documentoTransporte;

            notaFiscalDT.Emitente = this.ObterEmitente(reg501, documentoTransporte.Empresa.Codigo);

            notaFiscalDT.Numero = int.Parse(reg505.numNF);
            notaFiscalDT.Peso = reg505.registro507?.pTotTranspBruto ?? 0;

            notaFiscalDT.Quantidade = (int)(reg505.registro507?.qtdTotVolumes ?? 0);

            if (reg505.condFret == DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505CondFret.F)
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            else
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;

            notaFiscalDT.Serie = int.Parse(reg505.serieNF);
            notaFiscalDT.Valor = reg505.registro506?.vTotNota ?? 0;
            notaFiscalDT.ValorFrete = reg505.registro507?.vTotFret ?? 0;
            //notaFiscalDT.XML = dados.xmlNFe;

            repNotaFiscalDT.Inserir(notaFiscalDT);
        }

        private Dominio.Entidades.Cliente ObterDestinatario(DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503 reg503, int codigoEmpresa, string chaveNotaFiscal = "")
        {
            if (reg503 == null)
                return null;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(reg503.cnpjCpfDest));

            bool inserir = false;

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }
            else if (cliente.NaoAtualizarDados)
                return cliente;

            cliente.Bairro = (reg503.bairro ?? "").Length > 2 ? reg503.bairro : "Não Informado";
            cliente.CEP = reg503.codPostal;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = reg503.endereco;
            if (reg503.insEstadual != null && reg503.insEstadual != "")
                cliente.IE_RG = reg503.insEstadual;
            else
            {
                cliente.IE_RG = "ISENTO";
            }

            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(reg503.codMunicipio ?? "0")) ?? repLocalidade.BuscarPorDescricaoAbreviacaoUF(reg503.cidade ?? "", reg503.siglaEstado ?? "");
            if (localidade == null)
                throw new Exception("Localidade IBGE: " + (reg503.cidade ?? "") + " não encontrada.");

            cliente.Localidade = localidade;
            cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(reg503.razaoSocNmDest?.Replace("&amp;", "")?.Replace(" amp ", ""));
            //cliente.Numero = string.IsNullOrWhiteSpace(reg503.num) ? "S/N" : infNFeDest.enderDest.nro;
            cliente.Telefone1 = reg503.numComunic == null || reg503.numComunic.StartsWith("00") ? string.Empty : reg503.numComunic;
            cliente.Tipo = Utilidades.String.OnlyNumbers(cpfCnpj.ToString()).Length == 14 ? "J" : "F";

            if (cliente.Atividade == null)
                cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, _unitOfWork.StringConexao, 0, _unitOfWork);

            if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                cliente.IE_RG = "ISENTO";

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    if (!string.IsNullOrWhiteSpace(cliente.CPF_CNPJ_Formatado))
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                        if (grupoPessoas != null)
                        {
                            cliente.GrupoPessoas = grupoPessoas;
                        }
                    }
                }
                if (cliente.IE_RG == "ISENTO")
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                else
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                cliente.Ativo = true;
                cliente.AguardandoConferenciaInformacao = true;
                cliente.DataCadastro = System.DateTime.Now;
                cliente.DataUltimaAtualizacao = System.DateTime.Now;
                cliente.Integrado = false;
                repCliente.Inserir(cliente);
            }
            else
            {
                cliente.DataUltimaAtualizacao = System.DateTime.Now;
                cliente.Integrado = false;

                repCliente.Atualizar(cliente);
            }

            new Repositorio.Embarcador.Pessoas.PessoaIntegracao(_unitOfWork).GerarIntegracaoPessoa(_unitOfWork, cliente);

            return cliente;
        }

        private Dominio.Entidades.Cliente ObterEmitente(DT_NotfisDetail_SAPRegistro000Registro500Registro501 reg501, int codigoEmpresa)
        {
            if (reg501 == null)
                return null;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(reg501.cnpjEmbarcad ?? "0"));

            bool inserir = false;

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }
            else if (cliente.NaoAtualizarDados)
                return cliente;

            cliente.Bairro = reg501.bairro.Length > 2 ? reg501.bairro.Length > 40 ? reg501.bairro.Substring(0, 40) : reg501.bairro : "Não Informado";
            cliente.CEP = reg501.codPostal;
           
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = reg501.endereco;
            if (reg501.insEstEmbarcad != null && reg501.insEstEmbarcad != "")
                cliente.IE_RG = reg501.insEstEmbarcad;
            else
            {
                cliente.IE_RG = "ISENTO";
            }
            cliente.InscricaoMunicipal = reg501.insMunicipal;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(reg501.codMunicipio ?? "0")) ?? repLocalidade.BuscarPorDescricaoAbreviacaoUF(reg501.cidade ?? "", reg501.estado ?? "");
            cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(reg501.nmEmpEmbarcad?.Replace("&amp;", "")?.Replace(" amp ", ""));
            cliente.NomeFantasia = Utilidades.String.RemoverCaracteresEspecialSerpro(reg501.nmEmpEmbarcad?.Replace("&amp;", "")?.Replace(" amp ", ""));
           // cliente.Numero = string.IsNullOrWhiteSpace(infNFeEmit.enderEmit.nro) ? "S/N" : reg501.nu.nro;

            if (cliente.Numero == "0")
                cliente.Numero = "S/N";

            string telefone = (string.IsNullOrWhiteSpace(reg501.contatoEmergencias) || reg501.contatoEmergencias.StartsWith("00") ? string.Empty : reg501.contatoEmergencias);

            if (telefone.Length <= 15)
                cliente.Telefone1 = telefone;

            cliente.Tipo = Utilidades.String.OnlyNumbers(cpfCnpj.ToString()).Length == 14 ? "J" : "F";

            if (cliente.Atividade == null)
                cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, _unitOfWork.StringConexao, 0, _unitOfWork);

            if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                cliente.IE_RG = "ISENTO";

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                    if (grupoPessoas != null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                    }
                }
                if (cliente.IE_RG == "ISENTO")
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                else
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                cliente.Ativo = true;
                cliente.DataCadastro = DateTime.Now;
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Inserir(cliente);
            }
            else
            {
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Atualizar(cliente);
            }
            new Repositorio.Embarcador.Pessoas.PessoaIntegracao(_unitOfWork).GerarIntegracaoPessoa(_unitOfWork, cliente);

            return cliente;
        }

        private Servicos.Servico.Electrolux.NOTFIS.Detalhe.DT_ReturnNotfisDetail_SAP obterDadosXMLTeste()
        {
            Servicos.Servico.Electrolux.NOTFIS.Detalhe.DT_ReturnNotfisDetail_SAP retorno = new Servicos.Servico.Electrolux.NOTFIS.Detalhe.DT_ReturnNotfisDetail_SAP();

            // registro000.detail
            retorno.notfis = new DT_ReturnNotfisDetail_SAPNotfis();
            retorno.notfis.registro000 = new DT_NotfisDetail_SAPRegistro000();
            retorno.notfis.registro000.detail = new DT_Registro000_SAP();
            retorno.notfis.registro000.detail.identRemetente = "76487032003906";
            retorno.notfis.registro000.detail.identDest = "3831403000413";
            retorno.notfis.registro000.detail.data = Convert.ToDateTime("2025-06-05");
            retorno.notfis.registro000.detail.hora = Convert.ToDateTime("20:29:00");
            retorno.notfis.registro000.detail.identIntercambio = "NOT500506924";

            retorno.notfis.registro000.registro500 = new DT_NotfisDetail_SAPRegistro000Registro500[]
                {
                    new DT_NotfisDetail_SAPRegistro000Registro500()
                };
            retorno.notfis.registro000.registro500[0].identDocumento = "NOTAS500506924";

            // registro501
            retorno.notfis.registro000.registro500[0].registro501 = new DT_NotfisDetail_SAPRegistro000Registro500Registro501[]
            {
                new DT_NotfisDetail_SAPRegistro000Registro500Registro501()
            };

            retorno.notfis.registro000.registro500[0].registro501[0].nmEmpEmbarcad = "ELECTROLUX DO BRASIL S/A";
            retorno.notfis.registro000.registro500[0].registro501[0].cnpjEmbarcad = "76487032003906";

            retorno.notfis.registro000.registro500[0].registro501[0].endereco = "VEREADOR ÂNGELO BURBELLO";
            retorno.notfis.registro000.registro500[0].registro501[0].bairro = "CAMPO DE SANTANA";
            retorno.notfis.registro000.registro500[0].registro501[0].cidade = "CURITIBA";
            retorno.notfis.registro000.registro500[0].registro501[0].codPostal = "81945010";
            retorno.notfis.registro000.registro500[0].registro501[0].estado = "PR";

            // registro503
            retorno.notfis.registro000.registro500[0].registro501[0].registro503 = new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503[]
            {
                new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503()
            };

            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].razaoSocNmDest = "AMAZON SERVICOS DE VAREJO DO BRASIL";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].cnpjCpfDest = "15436940000871";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].insEstadual = "241134643114";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].endereco = "ACESSO NORTE KM 38,420 MZNINODO GLP 07 GLEBA A BL";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].bairro = "EMPRESARIAL GATO PRETO";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].cidade = "CAJAMAR";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].codPostal = "07789100";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].siglaEstado = "SP";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].tpIdentDest = DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503TpIdentDest.Item1;

            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505 = new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505[]
            {
                new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505()
            };

            // registro505
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].serieNF = "1";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].numNF = "000618616";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].dtEmissao = Convert.ToDateTime("2025-05-20");
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].naturezaMerc = "DIVERSOS";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].especieAcondic = "DIVERSOS";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].meioTransp = DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505MeioTransp.Item1;
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].tpTranspCarga = DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505TpTranspCarga.Item1;
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].tpCarga =DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505TpCarga.Item2;
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].condFret = DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505CondFret.C;
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].planoCargaRapida = DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505PlanoCargaRapida.N;
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].tpDocumentoFiscal = DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505TpDocumentoFiscal.Item1;
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].indicaBonificado = DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505IndicaBonificado.N;
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].codFiscalOperacao = "6102";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].estado = "00";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].calcFretDiferen = DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505CalcFretDiferen.N;
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].tabelaFret = "0";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].numCarregamento = "14977970";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].numPreCalc = "12787352000001";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].outroNumSap = "8219348734";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].tpPeriodoEntrega = DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505TpPeriodoEntrega.Item0;
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].dtInicialEntrega = Convert.ToDateTime("2025-05-25");
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].codChaveAcessoNfe = "012787352";
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].chaveAcessoNfe = "41250576487032003906550010006186161858093497";

            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro506 = new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505Registro506();
            // registro506
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro506.qtdTotVolumes = Convert.ToDecimal(1483.0);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro506.pBrutoTotMercNota = Convert.ToDecimal(7275.92);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro506.pLiquidoTotMercNota = Convert.ToDecimal(5843.0);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro506.pDensidCubagem = Convert.ToDecimal(58.881);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro506.incidenciaIcms =  DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505Registro506IncidenciaIcms.S;
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro506.seguroEfetuado = DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505Registro506SeguroEfetuado.S;
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro506.vTotNota = Convert.ToDecimal(533572.66);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro506.vTotSeguro = Convert.ToDecimal(533572.66);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro506.vCalculadoFret = Convert.ToDecimal(533572.66);

            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507 = new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505Registro507();
            // registro507
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.qtdTotVolumes = Convert.ToDecimal(1483.0);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.pTotTranspBruto = Convert.ToDecimal(7275.92);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.pTotCubado = Convert.ToDecimal(5843.0);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.pDensidCubagem = Convert.ToDecimal(58.881);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.vTotFret = Convert.ToDecimal(6725.38);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.vFretPVolume = Convert.ToDecimal(4379.53);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.fretAdValem = Convert.ToDecimal(48.02);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.vPedagio = Convert.ToDecimal(194.0);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.vTotDespExt = Convert.ToDecimal(715.91);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.bCalcApuIcmsFret = Convert.ToDecimal(6531.38);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.taxaIcmsFret = Convert.ToDecimal(12.0);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.vIcmsFret = Convert.ToDecimal(783.77);
            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro507.substituicaoTribu = DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505Registro507SubstituicaoTribu.Item2;

            retorno.notfis.registro000.registro500[0].registro501[0].registro503[0].registro505[0].registro511 = new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505Registro511[]
            {
                new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505Registro511(){ qtdVolumes = Convert.ToDecimal(174.0), cItemNf = "900920487", descItemNf = "A10N1 ASPIRADOR PO E AGUA 127", cfopItem = "6101" },
                new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505Registro511(){ qtdVolumes = Convert.ToDecimal(48.0), cItemNf = "900921213", descItemNf = "PCE20 PANELA PRESSAO ELETR 127", cfopItem = "6102" },
                new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505Registro511(){ qtdVolumes = Convert.ToDecimal(679.0), cItemNf = "900921480", descItemNf = "STK15 ASPIRADOR POWERSPEED 220", cfopItem = "6102" },
                new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505Registro511(){ qtdVolumes = Convert.ToDecimal(42.0), cItemNf = "955117641", descItemNf = "EGS20 VAPORIZADOR DE ROUPAS 220V", cfopItem = "6102" },
                new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505Registro511(){ qtdVolumes = Convert.ToDecimal(17.0), cItemNf = "900921646", descItemNf = "STK14W ASPIRADOR VERTICAL - 220V", cfopItem = "6102" },
                new DT_NotfisDetail_SAPRegistro000Registro500Registro501Registro503Registro505Registro511(){ qtdVolumes = Convert.ToDecimal(523.0), cItemNf = "955117671", descItemNf = "EAF90 AIR FRYER OVEN 127V", cfopItem = "6102" }

            };
            // registro511 - lista de itens


            retorno.notfis.registro000.registro500[0].registro519 = new DT_NotfisDetail_SAPRegistro000Registro500Registro519();
            // registro519
            retorno.notfis.registro000.registro500[0].registro519.identRegistro = "0";
            retorno.notfis.registro000.registro500[0].registro519.vTotDasNf = Convert.ToDecimal(533572.66);
            retorno.notfis.registro000.registro500[0].registro519.pBrutoTotNf = Convert.ToDecimal(7275.92);
            retorno.notfis.registro000.registro500[0].registro519.qtdTotVolumes = Convert.ToDecimal(1483.0);
            retorno.notfis.registro000.registro500[0].registro519.numNotas = "1";

            return retorno;
        }
        #endregion


    }

}
