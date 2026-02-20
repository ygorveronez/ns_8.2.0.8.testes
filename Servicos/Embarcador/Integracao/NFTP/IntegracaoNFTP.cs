using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using com.alianca.logisticsoperations.emp.transportplan;
using com.maersk.billableitemspostrequest;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;

namespace Servicos.Embarcador.Integracao.NFTP
{
    public class IntegracaoNFTP
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private DeliveryReport<Null, string> _deliveryHandler;
        private string _numeroBookingAtual;

        #endregion Atributos

        #region Construtores

        public IntegracaoNFTP(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores


        #region Métodos Públicos
        public void IntegrarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string url)
        {
            if (faturaIntegracao == null || faturaIntegracao.Fatura == null || faturaIntegracao.Fatura.Situacao == SituacaoFatura.EmFechamento)
                return;

            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repFaturaCTe = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();

            faturaIntegracao = repFaturaIntegracao.BuscarPorCodigo(faturaIntegracao.Codigo);

            bool faturaCancelamento = faturaIntegracao.Fatura.Situacao == SituacaoFatura.Cancelado || faturaIntegracao.Fatura.Situacao == SituacaoFatura.EmCancelamento;

            string topic = faturaCancelamento ? configuracaoIntegracaoEMP?.TopicEnvioIntegracaoNFTPEMP : configuracaoIntegracaoEMP?.TopicEnvioIntegracaoNFTPEMP;

            var tipoIntegracao = TipoIntegracaoEMP.CTeFatura;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);

                if (faturaCancelamento ? (!configuracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP) : (!configuracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP))
                    throw new ServicoException("Integração da Fatura não está ativada!");

                com.maersk.BillableItemsPostRequest billableItemsPostRequest = new com.maersk.BillableItemsPostRequest();
                var listaTipoPropostaNotaDebito = new List<TipoPropostaMultimodal> {
                    TipoPropostaMultimodal.NotaDebito,
                    TipoPropostaMultimodal.DetentionCabotagem,
                    TipoPropostaMultimodal.DemurrageCabotagem,
                    TipoPropostaMultimodal.FaturamentoContabilidade,
                    TipoPropostaMultimodal.NoShowCabotagem
                };

                var tipoProposta = faturaIntegracao.Fatura.TipoOperacao ?? null;
                if (tipoProposta is not null && (listaTipoPropostaNotaDebito.Contains(tipoProposta.TipoPropostaMultimodal)))
                {
                    billableItemsPostRequest = ConverterBillableItemsDebitNotePostRequest(repFaturaCTe.BuscarCTesPorFatura(faturaIntegracao.Fatura.Codigo), faturaIntegracao.Fatura, configuracaoIntegracaoEMP);
                }
                else if (faturaCancelamento)
                {
                    billableItemsPostRequest = ConverterBillableItemsPostRequestCancelamento(repFaturaCTe.BuscarCTesPorFatura(faturaIntegracao.Fatura.Codigo), faturaIntegracao.Fatura);
                }
                else
                    billableItemsPostRequest = ConverterBillableItemsPostRequest(repFaturaCTe.BuscarCTesPorFatura(faturaIntegracao.Fatura.Codigo), faturaIntegracao.Fatura);

                faturaIntegracao.DataEnvio = DateTime.Now;
                faturaIntegracao.Tentativas++;

                TipoIntegracaoEMP tipoIntegracaoEMP = billableItemsPostRequest.billableHeader.triggerType == "Debit Note Cancellation" ? TipoIntegracaoEMP.NdNFTP : TipoIntegracaoEMP.FaturaNFTP;

                EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, billableItemsPostRequest, out string arquivoEnvio, out string arquivoRetorno, tipoIntegracaoEMP, url);

                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                faturaIntegracao.MensagemRetorno = "Integrado com sucesso.";
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);

                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = excecao.Message;
                faturaIntegracao.Tentativas = 999;

                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, tipoIntegracao, _numeroBookingAtual, faturaIntegracao.Fatura.Numero.ToString());
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = "Ocorreu uma falha ao realizar a integração NFTP";
                faturaIntegracao.Tentativas = 999;

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar Fatura: {excecao.Message}", tipoIntegracao, _numeroBookingAtual, faturaIntegracao.Fatura.Numero.ToString());
            }
            try
            {
                repFaturaIntegracao.Atualizar(faturaIntegracao);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                faturaIntegracao = repFaturaIntegracao.BuscarPorCodigo(faturaIntegracao.Codigo);
                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = "Ocorreu uma falha ao realizar a integração NFTP.";
                repFaturaIntegracao.Atualizar(faturaIntegracao);
            }
        }
        public void IntegrarCartaCorrecaoCTe(Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao cartaCorrecaoIntegracao, string url)
        {
            Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao repCartaCorrecaoIntegracao = new Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoNFTPEMP;
            _numeroBookingAtual = cartaCorrecaoIntegracao.CartaCorrecao?.CTe?.NumeroBooking ?? "";

            cartaCorrecaoIntegracao.DataIntegracao = DateTime.Now;
            cartaCorrecaoIntegracao.NumeroTentativas++;

            var tipoIntegracao = TipoIntegracaoEMP.CartaCorrecao;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);
                if (!configuracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP)
                    throw new ServicoException("Integração de CCe não está ativada!");

                if ((configuracaoIntegracaoEMP?.ModificarConexaoParaEnvioRetina ?? false) && ((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCartaCorrecaoEMP ?? "") == "A") && ((configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoNavioEMP ?? false) || (configuracaoIntegracaoEMP?.AtivarIntegracaoComObjetoUnitoParaTodosTopics ?? false)))
                {
                    anl.documentation.CCetransport.correctionLetter cartaCorrecao = new anl.documentation.CCetransport.correctionLetter();

                    if (configuracaoIntegracaoEMP.TipoAVRO == TipoAVRO.V2)
                        cartaCorrecao = ConverterObjetoCartaCorrecaoV2(cartaCorrecaoIntegracao.CartaCorrecao);
                    else
                        cartaCorrecao = ConverterObjetoCartaCorrecaoRetina(cartaCorrecaoIntegracao.CartaCorrecao);

                    EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cartaCorrecao, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CartaCorrecao, url);

                    servicoArquivoTransacao.Adicionar(cartaCorrecaoIntegracao, arquivoEnvio, arquivoRetorno, "json", "Envio do CCe do CT-e " + cartaCorrecao.cte?.FirstOrDefault()?.documentNumber?.ToString() ?? "");
                }
                else
                {
                    dynamic cartaCorrecao = null;

                    if (configuracaoIntegracaoEMP.TipoAVRO == TipoAVRO.V2)
                        cartaCorrecao = ConverterObjetoCartaCorrecaoV2(cartaCorrecaoIntegracao.CartaCorrecao);
                    else
                        cartaCorrecao = ConverterObjetoCartaCorrecao(cartaCorrecaoIntegracao.CartaCorrecao);

                    if ((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCartaCorrecaoEMP ?? "") == "A")
                        EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cartaCorrecao, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CartaCorrecao, url);
                    else
                        EfetuarIntegracaoEMPCCe(configuracaoIntegracaoEMP, topic, cartaCorrecao, url);

                    servicoArquivoTransacao.Adicionar(cartaCorrecaoIntegracao, _deliveryHandler?.Message?.Value, _deliveryHandler?.Value, "json");

                    if (_deliveryHandler != null && _deliveryHandler.Status == PersistenceStatus.NotPersisted)
                        throw new ServicoException(_deliveryHandler.Error?.Reason, errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);
                }

                cartaCorrecaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cartaCorrecaoIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException excecao)
            {
                cartaCorrecaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cartaCorrecaoIntegracao.ProblemaIntegracao = excecao.Message;

                if (!string.IsNullOrWhiteSpace(cartaCorrecaoIntegracao.ProblemaIntegracao) && cartaCorrecaoIntegracao.ProblemaIntegracao.Length > 300)
                    cartaCorrecaoIntegracao.ProblemaIntegracao = cartaCorrecaoIntegracao.ProblemaIntegracao.Substring(0, 300);

                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, tipoIntegracao, cartaCorrecaoIntegracao.CartaCorrecao?.CTe?.NumeroBooking ?? "");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cartaCorrecaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cartaCorrecaoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração NFTP";

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar CCe: {excecao.Message}", tipoIntegracao, cartaCorrecaoIntegracao.CartaCorrecao?.CTe?.NumeroBooking ?? "");
            }

            repCartaCorrecaoIntegracao.Atualizar(cartaCorrecaoIntegracao);
        }
        public void IntegrarArquivoMercanteIntegracao(Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao arquivoMercanteIntegracao)
        {
            Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao repositorioArquivoMercanteIntegracao = new Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repositorioIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repositorioIntegracaoEMP.Buscar();

            arquivoMercanteIntegracao.DataIntegracao = DateTime.Now;
            arquivoMercanteIntegracao.NumeroTentativas++;

            try
            {
                ValidarConfiguracaoIntegracao(integracaoEMP);

                if (!(integracaoEMP?.AtivarIntegracaoNFTPEMP ?? false))
                {
                    arquivoMercanteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    arquivoMercanteIntegracao.ProblemaIntegracao = "Integração EMP não configurada!";
                    repositorioArquivoMercanteIntegracao.Atualizar(arquivoMercanteIntegracao);
                    return;
                }

                anl.documentation.CeMerchantNumber.CeMerchant objetoAvro = new anl.documentation.CeMerchantNumber.CeMerchant
                {
                    bookingNumber = arquivoMercanteIntegracao.ConhecimentoDeTransporteEletronico?.NumeroBooking ?? string.Empty,
                    CeMerchantNumber = arquivoMercanteIntegracao.ConhecimentoDeTransporteEletronico?.NumeroCEMercante ?? string.Empty,
                    cteAuthProtocol = arquivoMercanteIntegracao.ConhecimentoDeTransporteEletronico.Protocolo,
                    cteKey = arquivoMercanteIntegracao.ConhecimentoDeTransporteEletronico?.Chave ?? string.Empty,
                    cteSeriesNumber = arquivoMercanteIntegracao.ConhecimentoDeTransporteEletronico?.Serie?.Numero ?? 0,
                    documentNumber = arquivoMercanteIntegracao.ConhecimentoDeTransporteEletronico?.Numero.ToString() ?? string.Empty,
                    eventTimestamp = arquivoMercanteIntegracao.DataIntegracao.ToUniversalTime().ToUnixMillseconds(),
                    issueDatetime = DateTime.Now.ToUniversalTime().ToUnixMillseconds()
                };

                string topic = integracaoEMP?.TopicEnvioIntegracaoNFTPEMP;
                _numeroBookingAtual = objetoAvro.bookingNumber;

                EfetuarIntegracaoEMPRetina(integracaoEMP, topic, objetoAvro, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CEMercante, "");

                arquivoMercanteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                arquivoMercanteIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException ex)
            {
                arquivoMercanteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                arquivoMercanteIntegracao.ProblemaIntegracao = ex.Message;

                if (!string.IsNullOrWhiteSpace(arquivoMercanteIntegracao.ProblemaIntegracao) && arquivoMercanteIntegracao.ProblemaIntegracao.Length > 300)
                    arquivoMercanteIntegracao.ProblemaIntegracao = arquivoMercanteIntegracao.ProblemaIntegracao.Substring(0, 300);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                arquivoMercanteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                arquivoMercanteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do NFTP";
            }


            repositorioArquivoMercanteIntegracao.Atualizar(arquivoMercanteIntegracao);
        }
        public void IntegrarCargaCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaIntegracaoPendente, string url)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Servicos.WebService.CTe.CTe servicoWSCTe = new WebService.CTe.CTe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoNFTPEMP;

            cargaIntegracaoPendente.DataIntegracao = DateTime.Now;
            cargaIntegracaoPendente.NumeroTentativas++;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);
                if (!configuracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP)
                    throw new ServicoException("Integração da Carga Cancelamento não está ativada!");

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCargaCTe.BuscarDocumentosPorCarga(cargaIntegracaoPendente.CargaCancelamento.Carga.Codigo);

                foreach (var listaCTe in listaCTes)
                {
                    _numeroBookingAtual = listaCTe.NumeroBooking;
                    com.maersk.BillableItemsPostRequest cteAvroCancelamento = ConverterRetornoCTeAvroNFTP(listaCTe, listaCTe.Status == "C", false, null);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarPorCTe(listaCTe.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(cargaCte.Carga.Codigo);

                    var errors = servicoWSCTe.TratarErroHandlind(cteAvroCancelamento, listaCTe, cargaPedido);

                    if (errors.Any())
                        throw new ServicoException(string.Join(" ; ", errors), errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);

                    EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteAvroCancelamento, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CTeNFTP, url);
                    servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, _deliveryHandler?.Message?.Value, _deliveryHandler?.Value, "json");
                }

                if (_deliveryHandler != null && _deliveryHandler.Status == PersistenceStatus.NotPersisted)
                    throw new ServicoException(_deliveryHandler.Error?.Reason, errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);

                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaIntegracaoPendente.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException excecao)
            {
                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = excecao.Message;

                if (!string.IsNullOrWhiteSpace(cargaIntegracaoPendente.ProblemaIntegracao) && cargaIntegracaoPendente.ProblemaIntegracao.Length > 300)
                    cargaIntegracaoPendente.ProblemaIntegracao = cargaIntegracaoPendente.ProblemaIntegracao.Substring(0, 300);

                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, TipoIntegracaoEMP.Carga, _numeroBookingAtual);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração NFTP";

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar CT-e: {excecao.Message}", TipoIntegracaoEMP.Carga, _numeroBookingAtual);
            }

            repCargaIntegracao.Atualizar(cargaIntegracaoPendente);
        }
        public void IntegrarDadosCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracaoPendente, string url)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();

            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoNFTPEMP;
            string mensagem = string.Empty;

            integracaoPendente.DataIntegracao = DateTime.Now;
            integracaoPendente.NumeroTentativas++;

            string arquivoEnviado = string.Empty;
            string retornoExcecao = string.Empty;
            bool gerouArquivoHistorico = false;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);

                if (!configuracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP)
                    throw new ServicoException("Integração SIL não está ativada!");

                com.alianca.logisticsoperations.emp.transportplan.transportplan objetoAvro = ConverterCargaEmObjetoAVRO(integracaoPendente.Carga);
                arquivoEnviado = JsonConvert.SerializeObject(objetoAvro);
                EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, objetoAvro, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.DadosCarga, url);

                servicoArquivoTransacao.Adicionar(integracaoPendente, arquivoEnvio, arquivoRetorno, "json");
                gerouArquivoHistorico = true;

                if (_deliveryHandler != null && _deliveryHandler.Status == PersistenceStatus.NotPersisted)
                    throw new ServicoException(_deliveryHandler.Error?.Reason, errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);

                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracaoPendente.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException excecao)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = excecao.Message;

                if (!string.IsNullOrWhiteSpace(integracaoPendente.ProblemaIntegracao) && integracaoPendente.ProblemaIntegracao.Length > 300)
                    integracaoPendente.ProblemaIntegracao = integracaoPendente.ProblemaIntegracao.Substring(0, 300);

                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, TipoIntegracaoEMP.Carga, _numeroBookingAtual);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração EMP com SIL";
                retornoExcecao = excecao.Message;

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar Carga: {excecao.Message}", TipoIntegracaoEMP.DadosCarga, "");
            }

            if (!gerouArquivoHistorico)
                servicoArquivoTransacao.Adicionar(integracaoPendente, arquivoEnviado, retornoExcecao, "json");

            repCargaIntegracao.Atualizar(integracaoPendente);
        }
        public void IntegrarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracaoPendente, string url)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Servicos.WebService.CTe.CTe servicoWSCTe = new WebService.CTe.CTe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoNFTPEMP;

            ocorrenciaCTeIntegracaoPendente.DataIntegracao = DateTime.Now;
            ocorrenciaCTeIntegracaoPendente.NumeroTentativas++;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);
                if (!configuracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP)
                    throw new ServicoException("Integração da Ocorrência não está ativada!");

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(ocorrenciaCTeIntegracaoPendente.CargaOcorrencia.Codigo);
                foreach (var envioCTe in listaCTes)
                {
                    _numeroBookingAtual = envioCTe.NumeroBooking;
                    com.maersk.BillableItemsPostRequest cteRetorno = ConverterRetornoCTeAvroNFTP(envioCTe, false, true, ocorrenciaCTeIntegracaoPendente.CargaOcorrencia);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarPorCTe(envioCTe.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(cargaCte.Carga.Codigo);

                    var errors = servicoWSCTe.TratarErroHandlind(cteRetorno, envioCTe, cargaPedido);

                    if (errors.Any())
                        throw new ServicoException(string.Join(" ; ", errors), errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);

                    EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CTeComplementarNFTP, url);
                    servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracaoPendente, arquivoEnvio, arquivoRetorno, "json", "Envio da ocorrencia do CT-e " + envioCTe.Numero.ToString());
                }

                if (_deliveryHandler != null && _deliveryHandler.Status == PersistenceStatus.NotPersisted)
                    throw new ServicoException(_deliveryHandler.Error?.Reason, errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);

                ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException excecao)
            {
                ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = excecao.Message;

                if (!string.IsNullOrWhiteSpace(ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao) && ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao.Length > 300)
                    ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao.Substring(0, 300);

                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, TipoIntegracaoEMP.CTeComplementarNFTP, _numeroBookingAtual);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração NFTP";

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar CT-e: {excecao.Message}", TipoIntegracaoEMP.CTeComplementarNFTP, _numeroBookingAtual);
            }

            repOcorrenciaIntegracao.Atualizar(ocorrenciaCTeIntegracaoPendente);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoPendente, string url)
        {

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Servicos.WebService.CTe.CTe servicoWSCTe = new WebService.CTe.CTe(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();

            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoNFTPEMP;

            cargaIntegracaoPendente.DataIntegracao = DateTime.Now;
            cargaIntegracaoPendente.NumeroTentativas++;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);
                if (!configuracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP)
                    throw new ServicoException("Integração da Carga não está ativada!");

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCargaCTe.BuscarDocumentosPorCarga(cargaIntegracaoPendente.Carga.Codigo);

                foreach (var listaCTe in listaCTes)
                {
                    _numeroBookingAtual = listaCTe.NumeroBooking;
                    com.maersk.BillableItemsPostRequest cteAvroCancelamento = ConverterRetornoCTeAvroNFTP(listaCTe, listaCTe.Status == "C", false, null);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarPorCTe(listaCTe.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(cargaCte.Carga.Codigo);

                    var errors = servicoWSCTe.TratarErroHandlind(cteAvroCancelamento, listaCTe, cargaPedido);

                    if (errors.Any())
                        throw new ServicoException(string.Join(" ; ", errors), errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);

                    EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteAvroCancelamento, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CTeNFTP, url);
                    servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, _deliveryHandler?.Message?.Value, _deliveryHandler?.Value, "json");
                }

                if (_deliveryHandler != null && _deliveryHandler.Status == PersistenceStatus.NotPersisted)
                    throw new ServicoException(_deliveryHandler.Error?.Reason, errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);


                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaIntegracaoPendente.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException excecao)
            {
                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = excecao.Message;

                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, TipoIntegracaoEMP.CTeNFTP, _numeroBookingAtual);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração EMP NFTP";

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar CT-e NFTP: {excecao.Message}", TipoIntegracaoEMP.CTeNFTP, _numeroBookingAtual);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(cargaIntegracaoPendente.ProblemaIntegracao) && cargaIntegracaoPendente.ProblemaIntegracao.Length > 300)
                    cargaIntegracaoPendente.ProblemaIntegracao = cargaIntegracaoPendente.ProblemaIntegracao.Substring(0, 300);

                repCargaIntegracao.Atualizar(cargaIntegracaoPendente);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracaoPendente = repCargaIntegracao.BuscarPorCodigo(cargaIntegracaoPendente.Codigo);
                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração EMP NFTP";
                repCargaIntegracao.Atualizar(cargaIntegracaoPendente);
            }
        }
        public void IntegrarCargaCTeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaIntegracaoPendente, string url)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Servicos.WebService.CTe.CTe servicoWSCTe = new WebService.CTe.CTe(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoNFTPEMP;
            _numeroBookingAtual = cargaIntegracaoPendente.CTe?.NumeroBooking ?? "";

            cargaIntegracaoPendente.DataIntegracao = DateTime.Now;
            cargaIntegracaoPendente.NumeroTentativas++;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);
                if (!configuracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP)
                    throw new ServicoException("Integração do CT-e Manual não está ativada!");

                if (cargaIntegracaoPendente.CTe.Status == "A" && string.IsNullOrWhiteSpace(topic))
                    throw new ServicoException("Topic não configurado para a integração de CT-e manual NFTP!");

                _numeroBookingAtual = cargaIntegracaoPendente.CTe.NumeroBooking;
                com.maersk.BillableItemsPostRequest cteAvroCancelamento = ConverterRetornoCTeAvroNFTP(cargaIntegracaoPendente.CTe, cargaIntegracaoPendente.CTe.Status == "C", false, null);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarPorCTe(cargaIntegracaoPendente.CTe.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(cargaCte.Carga.Codigo);

                var errors = servicoWSCTe.TratarErroHandlind(cteAvroCancelamento, cargaIntegracaoPendente.CTe, cargaPedido);

                if (errors.Any())
                    throw new ServicoException(string.Join(" ; ", errors), errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);

                EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteAvroCancelamento, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CTeNFTP, url);
                servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, _deliveryHandler?.Message?.Value, _deliveryHandler?.Value, "json");

                if (_deliveryHandler != null && _deliveryHandler.Status == PersistenceStatus.NotPersisted)
                    throw new ServicoException(_deliveryHandler.Error?.Reason, errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);

                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaIntegracaoPendente.ProblemaIntegracao = "Integrado com sucesso.";

            }
            catch (ServicoException excecao)
            {
                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = excecao.Message;

                if (!string.IsNullOrWhiteSpace(cargaIntegracaoPendente.ProblemaIntegracao) && cargaIntegracaoPendente.ProblemaIntegracao.Length > 300)
                    cargaIntegracaoPendente.ProblemaIntegracao = cargaIntegracaoPendente.ProblemaIntegracao.Substring(0, 300);

                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, TipoIntegracaoEMP.CTeNFTP, cargaIntegracaoPendente.CTe.NumeroBooking);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração NFTP";

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar CT-e: {excecao.Message}", TipoIntegracaoEMP.CTeNFTP, cargaIntegracaoPendente.CTe.NumeroBooking);
            }

            repCargaIntegracao.Atualizar(cargaIntegracaoPendente);
        }
        public void IntegrarOcorrenciaCancelamento(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao ocorrenciaCancelamentoIntegracao, string url)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Servicos.WebService.CTe.CTe servicoWSCTe = new WebService.CTe.CTe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoNFTPEMP;

            ocorrenciaCancelamentoIntegracao.DataIntegracao = DateTime.Now;
            ocorrenciaCancelamentoIntegracao.NumeroTentativas++;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);
                if (!configuracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP)
                    throw new ServicoException("Integração do Cancelamento da Ocorrência não está ativada!");

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(ocorrenciaCancelamentoIntegracao.OcorrenciaCancelamento.Ocorrencia.Codigo);

                foreach (var cteEnvio in listaCTes)
                {
                    _numeroBookingAtual = cteEnvio.NumeroBooking;
                    com.maersk.BillableItemsPostRequest cteRetorno = ConverterRetornoCTeAvroNFTP(cteEnvio, true, true, ocorrenciaCancelamentoIntegracao.OcorrenciaCancelamento.Ocorrencia);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarPorCTe(cteEnvio.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(cargaCte.Carga.Codigo);

                    var errors = servicoWSCTe.TratarErroHandlind(cteRetorno, cteEnvio, cargaPedido);

                    if (errors.Any())
                        throw new ServicoException(string.Join(" ; ", errors), errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);

                    EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CTeComplementarNFTP, url);
                    servicoArquivoTransacao.Adicionar(ocorrenciaCancelamentoIntegracao, _deliveryHandler?.Error?.Reason, _deliveryHandler?.Value, "json", "Envio do CT-e " + _numeroBookingAtual);
                }

                if (_deliveryHandler != null && _deliveryHandler.Status == PersistenceStatus.NotPersisted)
                    throw new ServicoException(_deliveryHandler.Error?.Reason, errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);

                ocorrenciaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException excecao)
            {
                ocorrenciaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = excecao.Message;

                if (!string.IsNullOrWhiteSpace(ocorrenciaCancelamentoIntegracao.ProblemaIntegracao) && ocorrenciaCancelamentoIntegracao.ProblemaIntegracao.Length > 300)
                    ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = ocorrenciaCancelamentoIntegracao.ProblemaIntegracao.Substring(0, 300);

                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, TipoIntegracaoEMP.CTeComplementarNFTP, _numeroBookingAtual);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração NFTP";

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar CT-e: {excecao.Message}", TipoIntegracaoEMP.CTeComplementarNFTP, _numeroBookingAtual);
            }

            repOcorrenciaIntegracao.Atualizar(ocorrenciaCancelamentoIntegracao);
        }
        #endregion

        #region Métodos Privados
        private void ValidarConfiguracaoIntegracao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP)
        {
            if (configuracaoIntegracaoEMP == null || !configuracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP || string.IsNullOrWhiteSpace(configuracaoIntegracaoEMP.UsuarioEMP) || string.IsNullOrWhiteSpace(configuracaoIntegracaoEMP.SenhaEMP))
                throw new ServicoException("É necessário definir as configurações de integração EMP.");
        }
        private com.maersk.BillableItemsPostRequest ConverterBillableItemsPostRequest(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes, Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            List<com.maersk.billableitemspostrequest.BillableItems> billableItems = new List<BillableItems>();
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);

            foreach (var cte in listaCTes)
            {
                var tipoDocumento = cte.ModeloDocumentoFiscal?.TipoDocumentoEmissao;
                bool isNFS = tipoDocumento == Dominio.Enumeradores.TipoDocumento.NFS;

                com.maersk.billableitemspostrequest.BillableItems billableItem = new com.maersk.billableitemspostrequest.BillableItems
                {
                    collectionBusinessUnit = cte.Empresa.CodigoEmpresa,
                    triggerType = "Commercial Invoice",
                    cteNumber = isNFS ? null : cte.Numero.ToString(),
                    cteControlNumber = isNFS ? null : cte.NumeroControle,
                    accessKey = isNFS ? null : cte.Chave,
                    nfSeIssuanceNumber = isNFS ? cte.Numero.ToString() : null,
                    fiscalDocumentNumber = cte.Numero.ToString(),
                    commercialInvoiceNumber = fatura.Numero.ToString(),
                    boletoNumber = fatura.NumeroBoletos.ToString(),
                    invoiceDueDate = fatura.Parcelas?.FirstOrDefault()?.DataVencimento.ToString("yyyy-MM-dd"),
                    bookingNumber = !string.IsNullOrEmpty(cte?.NumeroBooking) ? cte.NumeroBooking : cte?.CargaCTes?.FirstOrDefault()?.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.NumeroBooking
                };

                billableItems.Add(billableItem);
            }

            com.maersk.BillableItemsPostRequest billableItemsPostRequest = new com.maersk.BillableItemsPostRequest
            {
                billableHeader = new com.maersk.billableitemspostrequest.BillableHeader
                {
                    sourceSystem = "MTMS",
                    triggerType = "Commercial Invoice",
                    messageCreationDatetime = serCTe.FormatarCampoDataAtualNFTP()
                },
                billableItems = billableItems,
                productSpecification = productSpecificationType.Generic
            };

            return billableItemsPostRequest;
        }
        private com.maersk.BillableItemsPostRequest ConverterBillableItemsPostRequestCancelamento(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes, Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            List<com.maersk.billableitemspostrequest.BillableItems> billableItems = new List<BillableItems>();
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);

            foreach (var cte in listaCTes)
            {
                var tipoDocumento = cte.ModeloDocumentoFiscal?.TipoDocumentoEmissao;
                bool isNFS = tipoDocumento == Dominio.Enumeradores.TipoDocumento.NFS;

                com.maersk.billableitemspostrequest.BillableItems billableItem = new com.maersk.billableitemspostrequest.BillableItems
                {
                    collectionBusinessUnit = cte?.Empresa.CodigoEmpresa,
                    triggerType = "Commercial Invoice Cancellation",
                    cteNumber = isNFS ? null : cte.Numero.ToString(),
                    cteControlNumber = isNFS ? null : cte.NumeroControle,
                    accessKey = isNFS ? null : cte.Chave,
                    nfSeIssuanceNumber = isNFS ? cte.Numero.ToString() : null,
                    fiscalDocumentNumber = cte?.Numero.ToString(),
                    commercialInvoiceNumber = "",
                    boletoNumber = "",
                    invoiceDueDate = "",
                    cancelReason = fatura?.MotivoCancelamento,
                    bookingNumber = !string.IsNullOrEmpty(cte?.NumeroBooking) ? cte.NumeroBooking : cte?.CargaCTes?.FirstOrDefault()?.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.NumeroBooking
                };

                billableItems.Add(billableItem);
            }

            com.maersk.BillableItemsPostRequest billableItemsPostRequest = new com.maersk.BillableItemsPostRequest
            {
                billableHeader = new com.maersk.billableitemspostrequest.BillableHeader
                {
                    sourceSystem = "MTMS",
                    triggerType = "Commercial Invoice Cancellation",
                    messageCreationDatetime = serCTe.FormatarCampoDataAtualNFTP()
                },
                billableItems = billableItems,
                productSpecification = productSpecificationType.Generic
            };

            return billableItemsPostRequest;
        }
        private com.maersk.BillableItemsPostRequest ConverterBillableItemsDebitNotePostRequest(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP)
        {
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.ComponentePrestacaoCTE repComponentePrestacaoCTE = new Repositorio.ComponentePrestacaoCTE(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamento = null;

            List<com.maersk.billableitemspostrequest.BillableItems> billableItems = new List<BillableItems>();
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);

            int sequencia = 0;

            foreach (var cte in listaCTes)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarPorCTe(cte.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(cargaCte.Carga.Codigo);

                if (cte.TomadorPagador != null && cte.TomadorPagador?.Cliente != null)
                    acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(cte.TomadorPagador.Cliente.CPF_CNPJ, 0);
                if (acordoFaturamento == null && cte.TomadorPagador != null && cte.TomadorPagador?.Cliente != null && cte.TomadorPagador?.GrupoPessoas != null)
                    acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(0, cte.TomadorPagador.GrupoPessoas.Codigo);

                var gerarFaturamentoAVista = acordoFaturamento?.CabotagemGerarFaturamentoAVista ?? false;

                sequencia++;

                foreach (var componente in cte.ComponentesPrestacao)
                {
                    com.maersk.billableitemspostrequest.BillableItems billableItem = new com.maersk.billableitemspostrequest.BillableItems();
                    var rawIdentifier = $"M{serCTe.FormatarCampoDataAtualNFTP()}{sequencia}";

                    billableItem.sourceSystemTransactionIdentifier = Regex.Replace(rawIdentifier, "[^a-zA-Z0-9]", "");
                    billableItem.chargeCode = componente?.Nome == "FRETE VALOR" ? integracaoEMP?.ComponenteFreteNFTPEMP?.ChargeId : componente?.Nome == "IMPOSTOS" ? integracaoEMP?.ComponenteImpostoNFTPEMP?.ChargeId : componente?.ComponenteFrete?.ChargeId ?? "";
                    billableItem.unitOfMeasurementCode = "CNT";
                    billableItem.isoCurrencyCode = "BRL";
                    billableItem.invoicingIsoCurrencyCode = "BRL";
                    billableItem.companyCode = "6000";
                    billableItem.triggerType = "Debit Note Creation";
                    billableItem.triggerDocumentType = "Debit Note";
                    billableItem.commercialInvoiceNumber = cte.Numero.ToString();
                    billableItem.invoiceDueDate = gerarFaturamentoAVista ? DateTime.Now.ToString("MM/dd/yyyy") : "";
                    billableItem.sourceSystemTransactionDate = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("MM/dd/yyyy") : "";
                    billableItem.timeOfOrigin = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("HH:mm:ss") : "";
                    billableItem.payToParty = cte?.TomadorPagador?.Cliente?.CMDID;
                    billableItem.portOfDischarge = cte?.PortoDestino?.RKST;
                    billableItem.portOfLoading = cte?.PortoOrigem?.RKST;
                    billableItem.revenueType = (bool)(cargaPedido?.Carga?.TipoOperacao?.ConfiguracaoEmissao?.TipoReceita.HasValue) ? TipoReceitaHelper.ObterDescricao(cargaPedido?.Carga?.TipoOperacao?.ConfiguracaoEmissao?.TipoReceita.Value) : "";
                    billableItem.baseRate = Convert.ToDouble(componente?.Valor);
                    billableItem.billingQuantity = cte.Containers?.Count();
                    billableItem.billableItemAmount = Convert.ToDouble(componente?.Valor);
                    billableItem.totalInvoiceAmount = Convert.ToDouble(cte?.ValorAReceber);
                    billableItem.collectionBusinessUnit = cte?.Empresa?.CodigoIntegracao;
                    billableItem.externalReference = cte?.Documentos?.FirstOrDefault().Numero;
                    billableItem.billToParty = cte?.TomadorPagador?.Cliente?.CMDID;
                    billableItem.vesselName = cte?.Navio?.Descricao ?? cte?.Viagem?.Navio?.Descricao ?? "";
                    billableItem.vesselCode = cte?.Viagem?.Navio?.CodigoNavio?.ToString();
                    billableItem.cardinalDirection = serCTe.ConverterDirecaoParaCardinalDirection(cte?.Viagem.DirecaoViagemMultimodal);
                    billableItem.voyageNumber = $"{cte?.Viagem.NumeroViagem}{cte?.Viagem.DirecaoViagemMultimodal.ObterAbreviacao()}";
                    billableItem.proposalType = cargaCte.Carga.TipoOperacao.Descricao;

                    var remarkSped = cargaPedido.Carga?.TipoOperacao?.ConfiguracaoCarga?.RemarkSped != null
                                        ? RemarkSpedHelper.ObterDescricao(cargaPedido.Carga.TipoOperacao.ConfiguracaoCarga.RemarkSped.Value) : "";
                    billableItem.pTSpedRemark = $"Receita de {remarkSped} {fatura?.Numero}";

                    if (fatura.Situacao is SituacaoFatura.EmCancelamento or SituacaoFatura.Cancelado)
                    {
                        billableItem.sourceSystemTransactionIdentifier = "";
                        billableItem.referenceSourceSystemTransactionIdentifier = componente?.SourceSystemTransactionIdentifier ?? "";
                        billableItem.cancelReason = fatura?.MotivoCancelamento;
                        billableItem.triggerType = "Debit Note Cancellation";
                    }

                    componente.SourceSystemTransactionIdentifier = billableItem.sourceSystemTransactionIdentifier ?? "";
                    repComponentePrestacaoCTE.Atualizar(componente);

                    billableItems.Add(billableItem);
                }
            }

            com.maersk.BillableItemsPostRequest billableItemsPostRequest = new com.maersk.BillableItemsPostRequest
            {
                billableHeader = new com.maersk.billableitemspostrequest.BillableHeader
                {
                    sourceSystem = "MTMS",
                    triggerType = fatura.Situacao is SituacaoFatura.EmCancelamento or SituacaoFatura.Cancelado ? "Debit Note Cancellation" : "Debit Note Creation",
                    messageCreationDatetime = serCTe.FormatarCampoDataAtualNFTP(),
                },
                productSpecification = productSpecificationType.Generic,
                billableItems = billableItems
            };

            return billableItemsPostRequest;
        }
        private void EfetuarIntegracaoEMPFatura(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP, string topic, object objetoJson, string url)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = configuracaoIntegracaoEMP.BoostrapServersEMP,
                SaslUsername = configuracaoIntegracaoEMP.UsuarioEMP,
                SaslPassword = configuracaoIntegracaoEMP.SenhaEMP,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                Acks = Acks.Leader,
                SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.Https
            };

            Headers headers = new Headers
            {
                { "Http-Connection", Encoding.ASCII.GetBytes(url) }
            };
            Message<Null, string> kafkaMessage = new Message<Null, string>()
            {
                Value = JsonConvert.SerializeObject(objetoJson),
                Headers = headers
            };

            using (IProducer<Null, string> producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(topic, kafkaMessage, HandlerFatura);

                producer.Flush(TimeSpan.FromSeconds(60));
            }

        }
        private void HandlerFatura(DeliveryReport<Null, string> deliveryReport)
        {
            _deliveryHandler = deliveryReport;

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");
#if DEBUG
            caminho = "D:\\Log";
#endif

            string caminhoArquivoEnvio = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoEnvio + ".json", deliveryReport.Message.Value);

            string caminhoArquivoRetorno = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoRetorno + ".json", deliveryReport.Value);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = caminhoArquivoEnvio,
                ArquivoRetorno = caminhoArquivoRetorno,
                DataEnvio = DateTime.Now,
                MensageRetorno = deliveryReport.Error?.Reason,
                StatusIntegracaoEMP = deliveryReport.Status == PersistenceStatus.NotPersisted ? StatusIntegracaoEMP.NotPersisted : deliveryReport.Status == PersistenceStatus.Persisted ? StatusIntegracaoEMP.Persisted : StatusIntegracaoEMP.PossiblyPersisted,
                Topic = deliveryReport.Topic,
                NumeroBooking = _numeroBookingAtual,
                TipoIntegracao = TipoIntegracaoEMP.CTeFatura
            };

            repIntegracaoEMPLog.Inserir(integracaoEMPLog);
        }
        private void RegistrarIntegracaoEMPLog(string topic, string mensagem, TipoIntegracaoEMP tipoIntegracao, string numeroBooking, string numeroFatura = null)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = "",
                ArquivoRetorno = "",
                DataEnvio = DateTime.Now,
                MensageRetorno = mensagem,
                StatusIntegracaoEMP = StatusIntegracaoEMP.NotPersisted,
                Topic = topic,
                SituacaoIntegracao = SituacaoIntegracaoEMP.NotPersist,
                Justificativa = mensagem,
                TipoIntegracao = tipoIntegracao,
                NumeroBooking = numeroBooking,
                Fatura = numeroFatura
            };

            repIntegracaoEMPLog.Inserir(integracaoEMPLog);
        }
        private void SalvarLogEMP(DeliveryResult<Confluent.Kafka.Null, string> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");
#if DEBUG
            caminho = "D:\\Log";
#endif

            string caminhoArquivoEnvio = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoEnvio + ".json", deliveryReport.Value);

            string caminhoArquivoRetorno = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoRetorno + ".json", deliveryReport.Message.Value);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = caminhoArquivoEnvio,
                ArquivoRetorno = caminhoArquivoRetorno,
                DataEnvio = DateTime.Now,
                MensageRetorno = deliveryReport.Message.Value,
                StatusIntegracaoEMP = StatusIntegracaoEMP.Persisted,
                Topic = deliveryReport.Topic,
                SituacaoIntegracao = SituacaoIntegracaoEMP.Integrado,
                TipoIntegracao = tipoIntegracao,
                NumeroBooking = numeroBooking
            };
            repIntegracaoEMPLog.Inserir(integracaoEMPLog);
        }


        private void SalvarLogEMP(DeliveryResult<string, com.maersk.BillableItemsPostRequest> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");
#if DEBUG
            caminho = "C:\\Log";
#endif

            string caminhoArquivoEnvio = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoEnvio + ".json", JsonConvert.SerializeObject(deliveryReport.Value));

            string caminhoArquivoRetorno = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoRetorno + ".json", JsonConvert.SerializeObject(deliveryReport.Message.Value));

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = caminhoArquivoEnvio,
                ArquivoRetorno = caminhoArquivoRetorno,
                DataEnvio = DateTime.Now,
                MensageRetorno = JsonConvert.SerializeObject(deliveryReport.Message.Value),
                StatusIntegracaoEMP = StatusIntegracaoEMP.Persisted,
                Topic = deliveryReport.Topic,
                SituacaoIntegracao = SituacaoIntegracaoEMP.Integrado,
                TipoIntegracao = tipoIntegracao,
                NumeroBooking = numeroBooking
            };
            repIntegracaoEMPLog.Inserir(integracaoEMPLog);
        }
        private void SalvarLogErroEMP(string msgErro, string topic, string jsonEnvio, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");
#if DEBUG
            caminho = "C:\\Log";
#endif
            string caminhoArquivoEnvio = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoEnvio + ".json", jsonEnvio);

            string caminhoArquivoRetorno = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoRetorno + ".txt", msgErro);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = caminhoArquivoEnvio,
                ArquivoRetorno = caminhoArquivoRetorno,
                DataEnvio = DateTime.Now,
                MensageRetorno = msgErro,
                StatusIntegracaoEMP = StatusIntegracaoEMP.NotPersisted,
                Topic = topic,
                SituacaoIntegracao = SituacaoIntegracaoEMP.NotPersist,
                TipoIntegracao = tipoIntegracao,
                NumeroBooking = numeroBooking
            };
            repIntegracaoEMPLog.Inserir(integracaoEMPLog);
        }
        private void EfetuarIntegracaoEMPRetina(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP, string topic, object objetoJson, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP tipoIntegracao, string url)
        {
            arquivoEnvio = "";
            arquivoRetorno = "";

            string caminhoCertificadoCa = (configuracaoIntegracaoEMP.CertificadoCRTServerRetina?.NomeArquivo ?? "");
            string caminhoCertificadoSsl = (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "");

#if DEBUG
            caminhoCertificadoCa = "C:\\Empresas\\caCorreto.crt";
            caminhoCertificadoSsl = "C:\\Empresas\\retinappschemaregistryv1.pfx";
#endif

            var jsonSerializerConfig = new JsonSerializerConfig
            {
                BufferBytes = 999999
            };

            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = configuracaoIntegracaoEMP.BootstrapServerRetina,
                SecurityProtocol = SecurityProtocol.SaslSsl,//comentario
                SaslMechanism = SaslMechanism.ScramSha512,
                SslCaLocation = caminhoCertificadoCa,
                SaslUsername = configuracaoIntegracaoEMP.UsuarioServerRetina,
                SaslPassword = configuracaoIntegracaoEMP.SenhaServerRetina
            };

            if (configuracaoIntegracaoEMP.ModificarConexaoParaEnvioRetina && configuracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP && (objetoJson.GetType() == typeof(com.maersk.BillableItemsPostRequest) || objetoJson.GetType().BaseType == typeof(com.maersk.BillableItemsPostRequest)))
            {
                var headers = new Headers();
                headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
                Message<string, com.maersk.BillableItemsPostRequest> kafkaMessage = new Message<string, com.maersk.BillableItemsPostRequest>()
                {
                    Key = "BillableItemsPostRequest",
                    Value = (com.maersk.BillableItemsPostRequest)objetoJson,
                    Headers = headers
                };

                arquivoEnvio = JsonConvert.SerializeObject(kafkaMessage.Value);

                using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                    new SchemaRegistryConfig
                    {
                        Url = configuracaoIntegracaoEMP.URLSchemaRegistryRetina,
                        SslKeystorePassword = configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina,//comentario
                        SslKeystoreLocation = caminhoCertificadoSsl,
                    }))
                using (IProducer<string, com.maersk.BillableItemsPostRequest> producer = new ProducerBuilder<string, com.maersk.BillableItemsPostRequest>(config)
                     .SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                     .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<com.maersk.BillableItemsPostRequest>(schemaRegistry).AsSyncOverAsync())
                     .SetLogHandler((_, msg) => Servicos.Log.TratarErro($"[LOG] {msg.Level}: {msg.Message}", "EnvioRetina"))
                     .SetErrorHandler((_, err) => Servicos.Log.TratarErro($"[ERROR] {err.Reason}", "EnvioRetina"))
                     .Build())
                {
                    try
                    {
                        var deliveryStatus = producer.ProduceAsync(topic, kafkaMessage).Result;
                        arquivoRetorno = JsonConvert.SerializeObject(deliveryStatus.Message.Value);
                        SalvarLogEMP(deliveryStatus, tipoIntegracao, _numeroBookingAtual);

                        producer.Flush(TimeSpan.FromSeconds(10));
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        arquivoRetorno = ex.Message;
                        if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                            arquivoRetorno += " " + ex.InnerException.Message;
                        SalvarLogErroEMP(ex.Message, topic, JsonConvert.SerializeObject(kafkaMessage.Value), tipoIntegracao, _numeroBookingAtual);

                        throw new ServicoException(ex.Message);
                    }
                }
            }
            else
            {
                var headers = new Headers();
                headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
                Message<Null, string> kafkaMessage = new Message<Null, string>()
                {
                    Value = JsonConvert.SerializeObject(objetoJson),
                    Headers = headers
                };
                arquivoEnvio = kafkaMessage.Value;

                using (IProducer<Null, string> producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    try
                    {
                        var deliveryStatus = producer.ProduceAsync(topic, kafkaMessage).Result;
                        arquivoRetorno = deliveryStatus.Message.Value;
                        SalvarLogEMP(deliveryStatus, tipoIntegracao, _numeroBookingAtual);

                        producer.Flush(TimeSpan.FromSeconds(10));
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        arquivoRetorno = ex.Message;
                        SalvarLogErroEMP(ex.Message, topic, kafkaMessage.Value, tipoIntegracao, _numeroBookingAtual);

                        throw new ServicoException(ex.Message);
                    }
                }
            }
        }
        private anl.documentation.CCetransport.correctionLetter ConverterObjetoCartaCorrecaoV2(Dominio.Entidades.CartaDeCorrecaoEletronica cartaCorrecao)
        {
            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(_unitOfWork);
            List<Dominio.Entidades.ItemCCe> itens = repItemCCe.BuscarPorCCe(cartaCorrecao.Codigo);

            string descricaoCorrecao = "";
            foreach (var item in itens)
                descricaoCorrecao += "- Campo: " + item.CampoAlterado.NomeCampo + " Novo Valor: " + item.ValorAlterado;

            string condicaoUsoCCe = @"De acordo com o Artigo 58 - B do ajuste SINIEF 02/08, com efeito a partir de 02/06/2008, fica permitida a utilização de carta de
                                correção, para regularização de erro ocorrido na emissão de documentos fiscais relativos à prestação de serviço de
                                transporte, desde que o erro não esteja relacionado com: I - as variáveis que determinam o valor do imposto tais como: base
                                de cálculo, alíquota, diferença de preço, quantidade, valor da prestação; II - a correção de dados cadastrais que implique
                                mudança do emitente, tomador, remetente ou do destinatário; III - a data de emissão ou de saída.";

            anl.documentation.CCetransport.correctionLetter correctionLetter = new anl.documentation.CCetransport.correctionLetter();
            correctionLetter.eventTimestamp = cartaCorrecao?.DataEmissao?.ToUnixMillseconds() ?? 0;
            correctionLetter.bookingNumber = cartaCorrecao.CTe.NumeroBooking;
            correctionLetter.transportDocumentStatusCode = "110110";
            correctionLetter.transportDocumentStatusName = "AUTHORIZED";
            correctionLetter.protocol = cartaCorrecao?.Protocolo ?? string.Empty;
            correctionLetter.issueDatetime = cartaCorrecao?.DataEmissao?.ToUnixMillseconds() ?? 0;
            correctionLetter.effectiveFromDatetime = cartaCorrecao?.DataEmissao?.ToUnixMillseconds() ?? 0;
            correctionLetter.cte = PreencherCTes(cartaCorrecao);
            correctionLetter.text = condicaoUsoCCe;
            correctionLetter.correctionLetterInfo = PreencherCorrectionLetterInfo(cartaCorrecao);

            return correctionLetter;
        }
        private List<anl.documentation.CCetransport.cteInfo> PreencherCTes(Dominio.Entidades.CartaDeCorrecaoEletronica cartaCorrecao)
        {
            List<anl.documentation.CCetransport.cteInfo> ctes = new List<anl.documentation.CCetransport.cteInfo>();

            ctes.Add(new anl.documentation.CCetransport.cteInfo()
            {
                documentNumber = cartaCorrecao.CTe.Numero.ToString(),
                serialNumber = cartaCorrecao.CTe.Serie?.Numero.ToString() ?? string.Empty,
                cteKey = cartaCorrecao.CTe.Chave ?? string.Empty,
                protocol = cartaCorrecao.CTe.Protocolo?.ToString() ?? string.Empty,
            });

            return ctes;
        }
        private List<anl.documentation.CCetransport.correctionLetterInfo> PreencherCorrectionLetterInfo(Dominio.Entidades.CartaDeCorrecaoEletronica cartaCorrecao)
        {
            Servicos.CCe svcCCe = new Servicos.CCe(_unitOfWork);
            Servicos.CTe svcCTe = new Servicos.CTe(_unitOfWork);
            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(_unitOfWork);
            List<Dominio.Entidades.ItemCCe> itens = repItemCCe.BuscarPorCCe(cartaCorrecao.Codigo);

            List<anl.documentation.CCetransport.correctionLetterInfo> listaCorretionLetterInfo = new List<anl.documentation.CCetransport.correctionLetterInfo>();

            Stream stream = null;
            byte[] data = null;
            if (cartaCorrecao.CTe != null)
            {
                data = svcCTe.ObterXMLAutorizacao(cartaCorrecao.CTe, _unitOfWork);
                if (data != null)
                    stream = new MemoryStream(data);
            }

            foreach (var item in itens)
            {
                string valorAnterior = "";
                if (data != null)
                {
                    stream = new MemoryStream(data);
                    if (!item.Descricao.ToLower().Contains("navio"))
                        valorAnterior = svcCCe.BuscarValorAnteriorXML(item.GrupoCampo, item.NomeCampo, item.NumeroItemAlterado, stream);
                    if (string.IsNullOrWhiteSpace(valorAnterior) && (item.Descricao.ToLower().Contains("lacre") || item.Descricao.ToLower().Contains("container")))
                    {
                        if (cartaCorrecao.CTe.Containers != null && cartaCorrecao.CTe.Containers.Count > 0)
                        {
                            valorAnterior = "";
                            foreach (var container in cartaCorrecao.CTe.Containers)
                            {
                                if (item.Descricao.ToLower().Contains("lacre"))
                                {
                                    if (container != null && !string.IsNullOrWhiteSpace(container.Lacre1))
                                        valorAnterior += " Lacre 1: " + container.Lacre1;
                                    if (container != null && !string.IsNullOrWhiteSpace(container.Lacre2))
                                        valorAnterior += " Lacre 2: " + container.Lacre2;
                                    if (container != null && !string.IsNullOrWhiteSpace(container.Lacre3))
                                        valorAnterior += " Lacre 3: " + container.Lacre3;
                                }
                                else if (item.Descricao.ToLower().Contains("container"))
                                {
                                    if (container != null && !string.IsNullOrWhiteSpace(container.Numero))
                                        valorAnterior += " Container: " + container.Numero;
                                    else if (container != null && string.IsNullOrWhiteSpace(container.Numero) && !string.IsNullOrWhiteSpace(container.Container.Numero))
                                        valorAnterior += " Container: " + container.Container.Numero;
                                }
                            }
                        }
                    }
                }
                listaCorretionLetterInfo.Add(new anl.documentation.CCetransport.correctionLetterInfo
                {
                    code = item.CampoAlterado.Codigo.ToString(),
                    fieldName = item.CampoAlterado.Descricao,
                    newValue = item.ValorAlterado,
                    currentValue = valorAnterior
                });
            }

            return listaCorretionLetterInfo;
        }
        private anl.documentation.CCetransport.correctionLetter ConverterObjetoCartaCorrecaoRetina(Dominio.Entidades.CartaDeCorrecaoEletronica cartaCorrecao)
        {
            string condicaoUsoCCe = @"De acordo com o Artigo 58 - B do ajuste SINIEF 02/08, com efeito a partir de 02/06/2008, fica permitida a utilização de carta de
                                correção, para regularização de erro ocorrido na emissão de documentos fiscais relativos à prestação de serviço de
                                transporte, desde que o erro não esteja relacionado com: I - as variáveis que determinam o valor do imposto tais como: base
                                de cálculo, alíquota, diferença de preço, quantidade, valor da prestação; II - a correção de dados cadastrais que implique
                                mudança do emitente, tomador, remetente ou do destinatário; III - a data de emissão ou de saída.";

            anl.documentation.CCetransport.correctionLetter cce = new anl.documentation.CCetransport.correctionLetter()
            {
                eventTimestamp = cartaCorrecao?.DataEmissao?.ToUnixMillseconds() ?? 0,
                transportDocumentStatusCode = "110110",
                transportDocumentStatusName = "AUTHORIZED",
                bookingNumber = cartaCorrecao.CTe.NumeroBooking,
                protocol = cartaCorrecao.Protocolo,
                issueDatetime = cartaCorrecao?.DataEmissao?.ToUnixMillseconds() ?? 0,
                effectiveFromDatetime = cartaCorrecao?.DataRetornoSefaz?.ToUnixMillseconds() ?? cartaCorrecao?.DataEmissao?.ToUnixMillseconds() ?? 0,
                cte = PreencherCTes(cartaCorrecao),
                correctionLetterInfo = PreencherCorrectionLetterInfo(cartaCorrecao),
                text = condicaoUsoCCe,
            };

            return cce;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CartaCorrecao ConverterObjetoCartaCorrecao(Dominio.Entidades.CartaDeCorrecaoEletronica cartaCorrecao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CartaCorrecao cce = new Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CartaCorrecao()
            {
                Evento = "Carta de Correção",
                NumeroBooking = cartaCorrecao.CTe.NumeroBooking,
                NumeroCTe = cartaCorrecao.CTe.Numero,
                SerieCTe = cartaCorrecao.CTe.Serie.Numero,
                CamposCCe = ConverterObjetoCamposCartaCorrecao(cartaCorrecao)
            };

            return cce;
        }
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CartaCorrecaoCampo> ConverterObjetoCamposCartaCorrecao(Dominio.Entidades.CartaDeCorrecaoEletronica cartaCorrecao)
        {
            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(_unitOfWork);

            List<Dominio.Entidades.ItemCCe> itensCCe = repItemCCe.BuscarPorCCe(cartaCorrecao.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CartaCorrecaoCampo> camposCCe = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CartaCorrecaoCampo>();

            foreach (Dominio.Entidades.ItemCCe item in itensCCe)
            {
                Dominio.Entidades.CampoCCe campoCCe = item.CampoAlterado;

                Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CartaCorrecaoCampo objetoCampoCce = new Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CartaCorrecaoCampo()
                {
                    NomeCampo = campoCCe.NomeCampo,
                    De = string.Empty,//não temos essa informação
                    Para = item.ValorAlterado
                };

                camposCCe.Add(objetoCampoCce);
            }

            return camposCCe;
        }
        private void EfetuarIntegracaoEMPCCe(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP, string topic, object objetoJson, string url)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = configuracaoIntegracaoEMP.BoostrapServersEMP,
                SaslUsername = configuracaoIntegracaoEMP.UsuarioEMP,
                SaslPassword = configuracaoIntegracaoEMP.SenhaEMP,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                Acks = Acks.Leader,
                SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.Https
            };

            var headers = new Headers();
            headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
            Message<Null, string> kafkaMessage = new Message<Null, string>()
            {
                Value = JsonConvert.SerializeObject(objetoJson),
                Headers = headers
            };

            using (IProducer<Null, string> producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(topic, kafkaMessage, HandlerCCe);

                producer.Flush(TimeSpan.FromSeconds(60));
            }
        }
        private void HandlerCCe(DeliveryReport<Null, string> deliveryReport)
        {
            _deliveryHandler = deliveryReport;

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");
#if DEBUG
            caminho = "D:\\Log";
#endif

            string caminhoArquivoEnvio = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoEnvio + ".json", deliveryReport.Message.Value);

            string caminhoArquivoRetorno = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoRetorno + ".json", deliveryReport.Value);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = caminhoArquivoEnvio,
                ArquivoRetorno = caminhoArquivoRetorno,
                DataEnvio = DateTime.Now,
                MensageRetorno = deliveryReport.Error?.Reason,
                StatusIntegracaoEMP = deliveryReport.Status == PersistenceStatus.NotPersisted ? StatusIntegracaoEMP.NotPersisted : deliveryReport.Status == PersistenceStatus.Persisted ? StatusIntegracaoEMP.Persisted : StatusIntegracaoEMP.PossiblyPersisted,
                Topic = deliveryReport.Topic,
                NumeroBooking = _numeroBookingAtual,
                TipoIntegracao = TipoIntegracaoEMP.CartaCorrecao
            };

            repIntegracaoEMPLog.Inserir(integracaoEMPLog);
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual ConverterRetornoCTeManual(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repCteGerado = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(_unitOfWork);

            Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento cteRelacao = repCteGerado.BuscarPorCTeOriginal(cte.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual cteIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual();
            if (cte != null)
                cteIntegracao = serCTe.ConverterObjetoCTeManual(cte, new List<Dominio.Entidades.CTeContaContabilContabilizacao>(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, _unitOfWork, false);

            return cteIntegracao;
        }
        private void EfetuarIntegracaoEMPCTeManual(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP, string topic, object objetoJson, string url)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = configuracaoIntegracaoEMP.BoostrapServersEMP,
                SaslUsername = configuracaoIntegracaoEMP.UsuarioEMP,
                SaslPassword = configuracaoIntegracaoEMP.SenhaEMP,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                Acks = Acks.Leader,
                SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.Https
            };

            var headers = new Headers();
            headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
            Message<Null, string> kafkaMessage = new Message<Null, string>()
            {
                Value = JsonConvert.SerializeObject(objetoJson),
                Headers = headers
            };

            using (IProducer<Null, string> producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(topic, kafkaMessage, HandlerCTeManual);

                producer.Flush(TimeSpan.FromSeconds(60));
            }
        }
        private void HandlerCTeManual(DeliveryReport<Null, string> deliveryReport)
        {
            _deliveryHandler = deliveryReport;

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");
#if DEBUG
            caminho = "D:\\Log";
#endif

            string caminhoArquivoEnvio = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoEnvio + ".json", deliveryReport.Message.Value);

            string caminhoArquivoRetorno = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoRetorno + ".json", deliveryReport.Value);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = caminhoArquivoEnvio,
                ArquivoRetorno = caminhoArquivoRetorno,
                DataEnvio = DateTime.Now,
                MensageRetorno = deliveryReport.Error?.Reason,
                StatusIntegracaoEMP = deliveryReport.Status == PersistenceStatus.NotPersisted ? StatusIntegracaoEMP.NotPersisted : deliveryReport.Status == PersistenceStatus.Persisted ? StatusIntegracaoEMP.Persisted : StatusIntegracaoEMP.PossiblyPersisted,
                Topic = deliveryReport.Topic,
                NumeroBooking = _numeroBookingAtual,
                TipoIntegracao = TipoIntegracaoEMP.CTeManual
            };

            repIntegracaoEMPLog.Inserir(integracaoEMPLog);
        }

        private void EfetuarIntegracaoEMPCTe(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP, string topic, object objetoJson, string url)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = configuracaoIntegracaoEMP.BoostrapServersEMP,
                SaslUsername = configuracaoIntegracaoEMP.UsuarioEMP,
                SaslPassword = configuracaoIntegracaoEMP.SenhaEMP,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                Acks = Acks.Leader,
                SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.Https
            };

            var headers = new Headers();
            headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
            Message<Null, string> kafkaMessage = new Message<Null, string>()
            {
                Value = JsonConvert.SerializeObject(objetoJson),
                Headers = headers
            };

            using (IProducer<Null, string> producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(topic, kafkaMessage, HandlerCTe);

                producer.Flush(TimeSpan.FromSeconds(60));
            }
        }
        private void HandlerCTe(DeliveryReport<Null, string> deliveryReport)
        {
            _deliveryHandler = deliveryReport;

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");
#if DEBUG
            caminho = "D:\\Log";
#endif

            string caminhoArquivoEnvio = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoEnvio + ".json", deliveryReport.Message.Value);

            string caminhoArquivoRetorno = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoRetorno + ".json", deliveryReport.Value);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = caminhoArquivoEnvio,
                ArquivoRetorno = caminhoArquivoRetorno,
                DataEnvio = DateTime.Now,
                MensageRetorno = deliveryReport.Error?.Reason,
                StatusIntegracaoEMP = deliveryReport.Status == PersistenceStatus.NotPersisted ? StatusIntegracaoEMP.NotPersisted : deliveryReport.Status == PersistenceStatus.Persisted ? StatusIntegracaoEMP.Persisted : StatusIntegracaoEMP.PossiblyPersisted,
                Topic = deliveryReport.Topic,
                NumeroBooking = _numeroBookingAtual,
                TipoIntegracao = TipoIntegracaoEMP.CTe
            };

            repIntegracaoEMPLog.Inserir(integracaoEMPLog);
        }
        private com.alianca.logisticsoperations.emp.transportplan.transportplan ConverterCargaEmObjetoAVRO(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault();

            com.alianca.logisticsoperations.emp.transportplan.transportplan objetoAvro = new transportplan();

            objetoAvro.startDatetime = cargaPedido.Pedido.DataPrevisaoSaida.HasValue ? cargaPedido.Pedido.DataPrevisaoSaida.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            objetoAvro.transportOrderNumber = cargaPedido.Pedido?.NumeroOS;
            objetoAvro.Driver = carga.Motoristas.FirstOrDefault()?.Nome ?? string.Empty;
            objetoAvro.identityNumber = carga.Motoristas.FirstOrDefault()?.CPF ?? string.Empty;
            objetoAvro.truckLicensePlate = carga.Veiculo?.Placa ?? string.Empty;
            objetoAvro.trailerLicensePlate = carga.VeiculosVinculados != null ? string.Join(", ", (from o in carga.VeiculosVinculados select o.Placa)) : string.Empty;
            objetoAvro.submittedDatetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            objetoAvro.protocol = carga.Protocolo > 0 ? carga.Protocolo.ToString() : string.Empty;

            return objetoAvro;
        }
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal> ConverterRetornoCTeNormal(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes)
        {
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal> CTesIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal>();

            if (listaCTes.Count > 0)
            {
                foreach (var cte in listaCTes)
                    CTesIntegracao.Add(serCTe.ConverterObjetoCTeNormal(cte, new List<Dominio.Entidades.CTeContaContabilContabilizacao>(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, _unitOfWork, false));
            }

            return CTesIntegracao;

        }
        private void EfetuarIntegracaoEMPCarga(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP, string topic, object objetoJson, string url)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = configuracaoIntegracaoEMP.BoostrapServersEMP,
                SaslUsername = configuracaoIntegracaoEMP.UsuarioEMP,
                SaslPassword = configuracaoIntegracaoEMP.SenhaEMP,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                Acks = Acks.Leader,
                SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.Https
            };

            var headers = new Headers();
            headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
            Message<Null, string> kafkaMessage = new Message<Null, string>()
            {
                Value = JsonConvert.SerializeObject(objetoJson),
                Headers = headers
            };

            using (IProducer<Null, string> producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(topic, kafkaMessage, HandlerCarga);

                producer.Flush(TimeSpan.FromSeconds(60));
            }
        }
        private void HandlerCarga(DeliveryReport<Null, string> deliveryReport)
        {
            _deliveryHandler = deliveryReport;

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");
#if DEBUG
            caminho = "D:\\Log";
#endif

            string caminhoArquivoEnvio = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoEnvio + ".json", deliveryReport.Message.Value);

            string caminhoArquivoRetorno = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoRetorno + ".json", deliveryReport.Value);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = caminhoArquivoEnvio,
                ArquivoRetorno = caminhoArquivoRetorno,
                DataEnvio = DateTime.Now,
                MensageRetorno = deliveryReport.Error?.Reason,
                StatusIntegracaoEMP = deliveryReport.Status == PersistenceStatus.NotPersisted ? StatusIntegracaoEMP.NotPersisted : deliveryReport.Status == PersistenceStatus.Persisted ? StatusIntegracaoEMP.Persisted : StatusIntegracaoEMP.PossiblyPersisted,
                Topic = deliveryReport.Topic,
                NumeroBooking = _numeroBookingAtual,
                TipoIntegracao = TipoIntegracaoEMP.Carga
            };

            repIntegracaoEMPLog.Inserir(integracaoEMPLog);
        }
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar> ConverterRetornoCTeComplementar(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes)
        {
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repCteComplementar = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar> listaCTesRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar>();

            if (ctes.Count > 0)
            {
                foreach (var cte in ctes)
                    listaCTesRetorno.Add(serCTe.ConverterObjetoCTeComplementar(cte, new List<Dominio.Entidades.CTeContaContabilContabilizacao>(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, _unitOfWork, false));
            }

            return listaCTesRetorno;

        }
        private void EfetuarIntegracaoEMPOcorrencia(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP, string topic, object objetoJson, string url)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = configuracaoIntegracaoEMP.BoostrapServersEMP,
                SaslUsername = configuracaoIntegracaoEMP.UsuarioEMP,
                SaslPassword = configuracaoIntegracaoEMP.SenhaEMP,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                Acks = Acks.Leader,
                SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.Https
            };

            var headers = new Headers();
            headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
            Message<Null, string> kafkaMessage = new Message<Null, string>()
            {
                Value = JsonConvert.SerializeObject(objetoJson),
                Headers = headers
            };

            using (IProducer<Null, string> producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(topic, kafkaMessage, HandlerOcorrencia);

                producer.Flush(TimeSpan.FromSeconds(60));
            }
        }
        private void HandlerOcorrencia(DeliveryReport<Null, string> deliveryReport)
        {
            _deliveryHandler = deliveryReport;

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");
#if DEBUG
            caminho = "D:\\Log";
#endif

            string caminhoArquivoEnvio = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoEnvio + ".json", deliveryReport.Message.Value);

            string caminhoArquivoRetorno = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoRetorno + ".json", deliveryReport.Value);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = caminhoArquivoEnvio,
                ArquivoRetorno = caminhoArquivoRetorno,
                DataEnvio = DateTime.Now,
                MensageRetorno = deliveryReport.Error?.Reason,
                StatusIntegracaoEMP = deliveryReport.Status == PersistenceStatus.NotPersisted ? StatusIntegracaoEMP.NotPersisted : deliveryReport.Status == PersistenceStatus.Persisted ? StatusIntegracaoEMP.Persisted : StatusIntegracaoEMP.PossiblyPersisted,
                Topic = deliveryReport.Topic,
                NumeroBooking = _numeroBookingAtual,
                TipoIntegracao = TipoIntegracaoEMP.Ocorrencia
            };

            repIntegracaoEMPLog.Inserir(integracaoEMPLog);
        }
        private com.maersk.BillableItemsPostRequest ConverterRetornoCTeAvroNFTP(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool cteCancelado, bool cteComplementar, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia? cargaOcorrencia)
        {
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);

            com.maersk.BillableItemsPostRequest cteAvroIntegracao = new com.maersk.BillableItemsPostRequest();
            if (cte != null)
                cteAvroIntegracao = serCTe.ConverterObjetoCTeAvroCarga(cte, _unitOfWork, cteCancelado, cteComplementar, cargaOcorrencia);

            return cteAvroIntegracao;
        }

        #endregion






    }
}
