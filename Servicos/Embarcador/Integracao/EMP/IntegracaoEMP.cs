using com.alianca.mtms.emp.responsectenormal;
using com.alianca.intercab.emp.doc.booking;
using com.maersk.customer.smds.commercial.msk;
using com.maersk.vessel.smds.operations.MSK;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Dominio.Entidades;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using Servicos.WebService.Carga;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Alianca.PushService.Domain.Models.Avro;
using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Configuracao;
using Dominio.Entidades.Embarcador.Cargas;
using com.alianca.logisticsoperations.emp.transportplan;
using System.Linq.Expressions;
using static Reflection;
using com.schedule.dto;
using Dominio.ObjetosDeValor.Embarcador.Integracao.EMP;
using Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.AVRO.Alianca;

namespace Servicos.Embarcador.Integracao.EMP
{
    public class IntegracaoEMP
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private DeliveryReport<Null, string> _deliveryHandler;
        private string _numeroBookingAtual;

        #endregion Atributos

        #region Construtores

        public IntegracaoEMP(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCartaCorrecaoCTe(Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao cartaCorrecaoIntegracao, string url)
        {
            Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao repCartaCorrecaoIntegracao = new Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCartaCorrecaoEMP;
            _numeroBookingAtual = cartaCorrecaoIntegracao.CartaCorrecao?.CTe?.NumeroBooking ?? "";

            cartaCorrecaoIntegracao.DataIntegracao = DateTime.Now;
            cartaCorrecaoIntegracao.NumeroTentativas++;

            var tipoIntegracao = TipoIntegracaoEMP.CartaCorrecao;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);
                if (!configuracaoIntegracaoEMP.AtivarIntegracaoCartaCorrecaoEMP)
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

                if (configuracaoIntegracaoEMP.AtivarEnvioIntegracaoBoletoEMP)
                {
                    tipoIntegracao = TipoIntegracaoEMP.Boleto;
                    BillingDocument billingDocument = ConverterBillingDocument(cartaCorrecaoIntegracao, _numeroBookingAtual);
                    EfetuarIntegracaoEMPCCe(configuracaoIntegracaoEMP, topic, billingDocument, url);
                }

                cartaCorrecaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cartaCorrecaoIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException excecao)
            {
                cartaCorrecaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cartaCorrecaoIntegracao.ProblemaIntegracao = excecao.Message;

                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, tipoIntegracao, cartaCorrecaoIntegracao.CartaCorrecao?.CTe?.NumeroBooking ?? "");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cartaCorrecaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cartaCorrecaoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração EMP";

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar CCe: {excecao.Message}", tipoIntegracao, cartaCorrecaoIntegracao.CartaCorrecao?.CTe?.NumeroBooking ?? "");
            }

            repCartaCorrecaoIntegracao.Atualizar(cartaCorrecaoIntegracao);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoPendente, string url)
        {
            Servicos.WebService.Carga.Carga serCarga = new WebService.Carga.Carga();

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();


            Servicos.WebService.CTe.CTe servicoWSCTe = new WebService.CTe.CTe(_unitOfWork);

            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCargaEMP;
            string topicDadosCarga = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoDadosCargaEMP;

            string mensagem = string.Empty;

            cargaIntegracaoPendente.DataIntegracao = DateTime.Now;
            cargaIntegracaoPendente.NumeroTentativas++;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);
                if (!configuracaoIntegracaoEMP.AtivarIntegracaoCargaEMP)
                    throw new ServicoException("Integração da Carga não está ativada!");

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCargaCTe.BuscarDocumentosPorCarga(cargaIntegracaoPendente.Carga.Codigo);

                if ((configuracaoIntegracaoEMP?.ModificarConexaoParaEnvioRetina ?? false) && ((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCargaEMP ?? "") == "A") && ((configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoNavioEMP ?? false) || (configuracaoIntegracaoEMP?.AtivarIntegracaoComObjetoUnitoParaTodosTopics ?? false)))
                {
                    foreach (var listaCTe in listaCTes)
                    {
                        _numeroBookingAtual = listaCTe.NumeroBooking;
                        anl.documentation.ctetransport.TransportDocumentationCTE cteRetorno = servicoWSCTe.ConverterObjetoCTeAvro(listaCTe, _unitOfWork);
                        EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.Carga, url);
                        servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, arquivoEnvio, arquivoRetorno, "json", "Envio do CT-e " + cteRetorno.documentNumber.ToString());
                    }
                }
                else
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal> listaCteRetorno = ConverterRetornoCTeNormal(listaCTes);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repCargaPedido.BuscarPorCarga(cargaIntegracaoPendente.Carga.Codigo);
                    List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasEnvio = serCarga.BuscarCargasPedidos(cargasPedido, configuracaoTMS, ref mensagem, _unitOfWork);

                    foreach (var cteRetorno in listaCteRetorno)
                    {

                        //EMPCTeNormal cteteste = new EMPCTeNormal();
                        //cteteste.Chave = "123";

                        //CTeNormal cteteste = new CTeNormal();
                        //cteteste.ChaveCTe = "123";

                        _numeroBookingAtual = cteRetorno.NumeroBooking;
                        if (((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCargaEMP ?? "") == "A"))
                            EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.Carga, url);
                        else
                            EfetuarIntegracaoEMPCTe(configuracaoIntegracaoEMP, topic, cteRetorno, url);
                        servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, _deliveryHandler?.Error?.Reason, _deliveryHandler?.Value, "json", "Envio do CT-e " + cteRetorno.Numero.ToString());
                    }

                    foreach (var cargaEnvio in cargasEnvio)
                    {
                        if (((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCargaEMP ?? "") == "A"))
                            EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topicDadosCarga, cargaEnvio, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.Carga, url);
                        else
                            EfetuarIntegracaoEMPCarga(configuracaoIntegracaoEMP, topicDadosCarga, cargaEnvio, url);
                        servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, _deliveryHandler?.Error?.Reason, _deliveryHandler?.Value, "json", "Envio da Carga " + cargaEnvio.NumeroCarga + " Container " + (cargaEnvio.Container?.Numero ?? ""));
                    }
                }

                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaIntegracaoPendente.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException excecao)
            {
                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = excecao.Message;

                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, TipoIntegracaoEMP.Carga, _numeroBookingAtual);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração EMP";

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar CT-e: {excecao.Message}", TipoIntegracaoEMP.Carga, _numeroBookingAtual);
            }

            repCargaIntegracao.Atualizar(cargaIntegracaoPendente);
        }

        public void IntegrarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracaoPendente, string url)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

            Servicos.WebService.CTe.CTe servicoWSCTe = new WebService.CTe.CTe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoOcorrenciaEMP;

            ocorrenciaCTeIntegracaoPendente.DataIntegracao = DateTime.Now;
            ocorrenciaCTeIntegracaoPendente.NumeroTentativas++;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);
                if (!configuracaoIntegracaoEMP.AtivarIntegracaoOcorrenciaEMP)
                    throw new ServicoException("Integração da Ocorrência não está ativada!");

                if ((configuracaoIntegracaoEMP?.ModificarConexaoParaEnvioRetina ?? false) && ((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoOcorrenciaEMP ?? "") == "A") && ((configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoNavioEMP ?? false) || (configuracaoIntegracaoEMP?.AtivarIntegracaoComObjetoUnitoParaTodosTopics ?? false)))
                {
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(ocorrenciaCTeIntegracaoPendente.CargaOcorrencia.Codigo);
                    foreach (var envioCTe in listaCTes)
                    {
                        _numeroBookingAtual = envioCTe.NumeroBooking;
                        anl.documentation.ctetransport.TransportDocumentationCTE cteRetorno = servicoWSCTe.ConverterObjetoCTeAvro(envioCTe, _unitOfWork);
                        EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.Ocorrencia, url);
                        servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracaoPendente, arquivoEnvio, arquivoRetorno, "json", "Envio da ocorrencia do CT-e " + cteRetorno.documentNumber.ToString());
                    }
                }
                else
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar> CTesRetorno = ConverterRetornoCTeComplementar(repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(ocorrenciaCTeIntegracaoPendente.CargaOcorrencia.Codigo));

                    foreach (var cteRetorno in CTesRetorno)
                    {
                        _numeroBookingAtual = cteRetorno.NumeroBooking;
                        if ((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoOcorrenciaEMP ?? "") == "A")
                            EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.Ocorrencia, url);
                        else
                            EfetuarIntegracaoEMPOcorrencia(configuracaoIntegracaoEMP, topic, cteRetorno, url);
                        servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracaoPendente, _deliveryHandler?.Error?.Reason, _deliveryHandler?.Value, "json", "Envio do CT-e " + cteRetorno.Numero.ToString());
                    }
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

                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, TipoIntegracaoEMP.Ocorrencia, _numeroBookingAtual);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração EMP";

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar CT-e: {excecao.Message}", TipoIntegracaoEMP.Ocorrencia, _numeroBookingAtual);
            }

            repOcorrenciaIntegracao.Atualizar(ocorrenciaCTeIntegracaoPendente);
        }

        public void IntegrarCargaCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaIntegracaoPendente, string url)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);


            Servicos.WebService.CTe.CTe servicoWSCTe = new WebService.CTe.CTe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCancelamentoCargaEMP;

            cargaIntegracaoPendente.DataIntegracao = DateTime.Now;
            cargaIntegracaoPendente.NumeroTentativas++;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);
                if (!configuracaoIntegracaoEMP.AtivarIntegracaoCancelamentoCargaEMP)
                    throw new ServicoException("Integração da Carga Cancelamento não está ativada!");

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCargaCTe.BuscarDocumentosPorCarga(cargaIntegracaoPendente.CargaCancelamento.Carga.Codigo);

                if ((configuracaoIntegracaoEMP?.ModificarConexaoParaEnvioRetina ?? false) && ((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCancelamentoCargaEMP ?? "") == "A") && ((configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoNavioEMP ?? false) || (configuracaoIntegracaoEMP?.AtivarIntegracaoComObjetoUnitoParaTodosTopics ?? false)))
                {
                    foreach (var cte in listaCTes)
                    {
                        _numeroBookingAtual = cte.NumeroBooking;
                        anl.documentation.ctetransport.TransportDocumentationCTE cteRetorno = servicoWSCTe.ConverterObjetoCTeAvro(cte, _unitOfWork);
                        EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.Carga, url);
                        servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, arquivoEnvio, arquivoRetorno, "json", "Envio do CT-e " + cteRetorno.documentNumber.ToString());
                    }
                }
                else
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento> listaCteRetorno = ConverterRetornoCTeCancelamento(listaCTes);

                    foreach (var cteRetorno in listaCteRetorno)
                    {
                        _numeroBookingAtual = cteRetorno.NumeroBooking;
                        if ((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCancelamentoCargaEMP ?? "") == "A")
                            EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.Carga, url);
                        else
                            EfetuarIntegracaoEMPCTe(configuracaoIntegracaoEMP, topic, cteRetorno, url);
                        servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, _deliveryHandler?.Error?.Reason, _deliveryHandler?.Value, "json", "Envio do CT-e " + cteRetorno.Numero.ToString());
                    }
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
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, TipoIntegracaoEMP.Carga, _numeroBookingAtual);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração EMP";

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar CT-e: {excecao.Message}", TipoIntegracaoEMP.Carga, _numeroBookingAtual);
            }

            repCargaIntegracao.Atualizar(cargaIntegracaoPendente);
        }

        public void IntegrarCargaCTeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaIntegracaoPendente, string url)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Servicos.WebService.CTe.CTe servicoWSCTe = new WebService.CTe.CTe(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCancelamentoCargaEMP;
            string topicCancelamento = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCancelamentoCTeManualEMP;
            _numeroBookingAtual = cargaIntegracaoPendente.CTe?.NumeroBooking ?? "";

            cargaIntegracaoPendente.DataIntegracao = DateTime.Now;
            cargaIntegracaoPendente.NumeroTentativas++;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);
                if (!configuracaoIntegracaoEMP.AtivarIntegracaoCTeManualEMP)
                    throw new ServicoException("Integração do CT-e Manual não está ativada!");

                if (cargaIntegracaoPendente.CTe.Status == "A" && string.IsNullOrWhiteSpace(topic))
                    throw new ServicoException("Topic não configurado para a integração de CT-e manual!");

                if (cargaIntegracaoPendente.CTe.Status != "A" && string.IsNullOrWhiteSpace(topicCancelamento))
                    throw new ServicoException("Topic não configurado para a integração de cancelamento do CT-e manual!");

                if (cargaIntegracaoPendente.CTe.Status != "A")
                    topic = topicCancelamento;

                if ((configuracaoIntegracaoEMP?.ModificarConexaoParaEnvioRetina ?? false) && ((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCTeManualEMP ?? "") == "A") && ((configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoNavioEMP ?? false) || (configuracaoIntegracaoEMP?.AtivarIntegracaoComObjetoUnitoParaTodosTopics ?? false)))
                {
                    anl.documentation.ctetransport.TransportDocumentationCTE cteRetorno = servicoWSCTe.ConverterObjetoCTeAvro(cargaIntegracaoPendente.CTe, _unitOfWork);
                    EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CTe, url);
                    servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, arquivoEnvio, arquivoRetorno, "json");
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual cteRetorno = ConverterRetornoCTeManual(cargaIntegracaoPendente.CTe);
                    if (((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCTeManualEMP ?? "") == "A"))
                        EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CTe, url);
                    else
                        EfetuarIntegracaoEMPCTeManual(configuracaoIntegracaoEMP, topic, cteRetorno, url);

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
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, TipoIntegracaoEMP.CTeManual, cargaIntegracaoPendente.CTe.NumeroBooking);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração EMP";

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar CT-e: {excecao.Message}", TipoIntegracaoEMP.CTeManual, cargaIntegracaoPendente.CTe.NumeroBooking);
            }

            repCargaIntegracao.Atualizar(cargaIntegracaoPendente);
        }

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

            string topic = faturaCancelamento ? configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCancelamentoFaturaEMP : configuracaoIntegracaoEMP?.TopicEnvioIntegracaoFaturaEMP;

            var tipoIntegracao = TipoIntegracaoEMP.CTeFatura;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);

                if (faturaCancelamento ? (!configuracaoIntegracaoEMP.AtivarIntegracaoCancelamentoFaturaEMP) : (!configuracaoIntegracaoEMP.AtivarIntegracaoFaturaEMP))
                    throw new ServicoException("Integração da Fatura não está ativada!");

                List<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura> listaCTes = ConverterRetornoCTes(repFaturaCTe.BuscarCTesPorFatura(faturaIntegracao.Fatura.Codigo), faturaIntegracao.Fatura);

                faturaIntegracao.DataEnvio = DateTime.Now;
                faturaIntegracao.Tentativas++;

                foreach (var cteRetorno in listaCTes)
                {
                    _numeroBookingAtual = cteRetorno.NumeroBooking;

                    if ((configuracaoIntegracaoEMP?.ModificarConexaoParaEnvioRetina ?? false) && ((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoFaturaEMP ?? "") == "A") && ((configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoNavioEMP ?? false) || (configuracaoIntegracaoEMP?.AtivarIntegracaoComObjetoUnitoParaTodosTopics ?? false)))
                    {
                        EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CTeFatura, url);
                    }
                    else
                    {
                        if ((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoFaturaEMP ?? "") == "A")
                            EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CTeFatura, url);
                        else
                        {
                            EfetuarIntegracaoEMPFatura(configuracaoIntegracaoEMP, topic, cteRetorno, url);

                            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo
                            {
                                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(_deliveryHandler?.Message?.Value, "json", _unitOfWork),
                                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(_deliveryHandler?.Value, "json", _unitOfWork),
                                Data = DateTime.Now,
                                Mensagem = faturaIntegracao.MensagemRetorno + ". Envio do CT-e " + cteRetorno.Numero.ToString(),
                                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                            };
                            repFaturaIntegracaoArquivo.Inserir(arquivoIntegracao);

                            faturaIntegracao = repFaturaIntegracao.BuscarPorCodigo(faturaIntegracao.Codigo);

                            if (faturaIntegracao.ArquivosIntegracao == null)
                                faturaIntegracao.ArquivosIntegracao = new List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo>();
                            faturaIntegracao.ArquivosIntegracao.Add(arquivoIntegracao);

                            if (_deliveryHandler != null && _deliveryHandler.Status == PersistenceStatus.NotPersisted)
                                throw new ServicoException(_deliveryHandler.Error?.Reason, errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);
                        }
                    }
                }


                if (configuracaoIntegracaoEMP.EnviarNoLayoutAvroDoPortalEMP)
                {
                    tipoIntegracao = TipoIntegracaoEMP.Fatura;
                    com.alianca.mtms.emp.billinginvoice.Invoice invoice = ConverterInvoice(faturaIntegracao, _numeroBookingAtual);
                    EfetuarIntegracaoEMPFatura(configuracaoIntegracaoEMP, topic, invoice, url);
                }

                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                faturaIntegracao.MensagemRetorno = "Integrado com sucesso.";
            }
            catch (ServicoException excecao)
            {
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
                faturaIntegracao.MensagemRetorno = "Ocorreu uma falha ao realizar a integração EMP";
                faturaIntegracao.Tentativas = 999;

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar Fatura: {excecao.Message}", tipoIntegracao, _numeroBookingAtual, faturaIntegracao.Fatura.Numero.ToString());
            }
            try
            {
                repFaturaIntegracao.Atualizar(faturaIntegracao);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegrarFatura");

                faturaIntegracao = repFaturaIntegracao.BuscarPorCodigo(faturaIntegracao.Codigo);
                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                faturaIntegracao.MensagemRetorno = "Integrado com sucessso.";
                repFaturaIntegracao.Atualizar(faturaIntegracao);
            }
        }

        public void IntegrarOcorrenciaCancelamento(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao ocorrenciaCancelamentoIntegracao, string url)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);


            Servicos.WebService.CTe.CTe servicoWSCTe = new WebService.CTe.CTe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCancelamentoOcorrenciaEMP;

            ocorrenciaCancelamentoIntegracao.DataIntegracao = DateTime.Now;
            ocorrenciaCancelamentoIntegracao.NumeroTentativas++;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);
                if (!configuracaoIntegracaoEMP.AtivarIntegracaoCancelamentoOcorrenciaEMP)
                    throw new ServicoException("Integração do Cancelamento da Ocorrência não está ativada!");

                if ((configuracaoIntegracaoEMP?.ModificarConexaoParaEnvioRetina ?? false) && ((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCancelamentoOcorrenciaEMP ?? "") == "A") && ((configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoNavioEMP ?? false) || (configuracaoIntegracaoEMP?.AtivarIntegracaoComObjetoUnitoParaTodosTopics ?? false)))
                {
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(ocorrenciaCancelamentoIntegracao.OcorrenciaCancelamento.Ocorrencia.Codigo);
                    foreach (var cteEnvio in listaCTes)
                    {
                        _numeroBookingAtual = cteEnvio.NumeroBooking;

                        anl.documentation.ctetransport.TransportDocumentationCTE cteRetorno = servicoWSCTe.ConverterObjetoCTeAvro(cteEnvio, _unitOfWork);
                        EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.Ocorrencia, url);
                        servicoArquivoTransacao.Adicionar(ocorrenciaCancelamentoIntegracao, arquivoEnvio, arquivoRetorno, "json", "Envio do CT-e " + cteRetorno.documentNumber.ToString());
                    }
                }
                else
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar> CTesRetorno = ConverterRetornoCTeComplementar(repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(ocorrenciaCancelamentoIntegracao.OcorrenciaCancelamento.Ocorrencia.Codigo));

                    foreach (var cteRetorno in CTesRetorno)
                    {
                        _numeroBookingAtual = cteRetorno.NumeroBooking;

                        if (((configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCancelamentoOcorrenciaEMP ?? "") == "A"))
                            EfetuarIntegracaoEMPRetina(configuracaoIntegracaoEMP, topic, cteRetorno, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.Ocorrencia, url);
                        else
                            EfetuarIntegracaoEMPOcorrencia(configuracaoIntegracaoEMP, topic, cteRetorno, url);
                        servicoArquivoTransacao.Adicionar(ocorrenciaCancelamentoIntegracao, _deliveryHandler?.Error?.Reason, _deliveryHandler?.Value, "json", "Envio do CT-e " + cteRetorno.Numero.ToString());
                    }
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

                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    RegistrarIntegracaoEMPLog(topic, excecao.Message, TipoIntegracaoEMP.Ocorrencia, _numeroBookingAtual);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração EMP";

                RegistrarIntegracaoEMPLog(topic, $"Falha ao integrar CT-e: {excecao.Message}", TipoIntegracaoEMP.Ocorrencia, _numeroBookingAtual);
            }

            repOcorrenciaIntegracao.Atualizar(ocorrenciaCancelamentoIntegracao);
        }

        public void IntegrarDadosCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracaoPendente, string url)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();

            string topic = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoParaSILEMP;
            string mensagem = string.Empty;

            integracaoPendente.DataIntegracao = DateTime.Now;
            integracaoPendente.NumeroTentativas++;

            string arquivoEnviado = string.Empty;
            string retornoExcecao = string.Empty;
            bool gerouArquivoHistorico = false;

            try
            {
                ValidarConfiguracaoIntegracao(configuracaoIntegracaoEMP);

                if (!configuracaoIntegracaoEMP.AtivarIntegracaoParaSILEMP)
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

                if (!(integracaoEMP?.AtivarIntegracaoCEMercanteEMP ?? false))
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

                string topic = integracaoEMP?.TopicEnvioIntegracaoCEMercanteEMP;
                _numeroBookingAtual = objetoAvro.bookingNumber;

                EfetuarIntegracaoEMPRetina(integracaoEMP, topic, objetoAvro, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP.CEMercante, "");

                arquivoMercanteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                arquivoMercanteIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException ex)
            {
                arquivoMercanteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                arquivoMercanteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                arquivoMercanteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                arquivoMercanteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do EMP";
            }

            //servicoArquivoTransacao.Adicionar(arquivoMercanteIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repositorioArquivoMercanteIntegracao.Atualizar(arquivoMercanteIntegracao);
        }

        public void RecebimentoBooking(string topic, IntercabDocBooking booking, out string msgRetorno, Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Log.TratarErro("Iniciando a interpretação do booking " + (booking?.bookingNumber ?? ""), "Booking");

            WebService.Carga.Pedido servicoPedidoWS = new WebService.Carga.Pedido(_unitOfWork);
            WebService.Carga.Carga servicoCargaWS = new WebService.Carga.Carga(_unitOfWork);
            ProdutosPedido servicoProdutosPedido = new ProdutosPedido(_unitOfWork);
            Carga.CargaIntegracao servicoCargaIntegracao = new Carga.CargaIntegracao();

            msgRetorno = "";
            StringBuilder mensagemErro = new StringBuilder();
            int protocoloCargaExistente = 0;
            int protocoloPedidoExistente = 0;

            try
            {
                ValidarCamposRecebimentoBooking(booking);

                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab configuracaoIntegracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repIntegracaoEMP.BuscarPrimeiroRegistro();

                Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino = repPorto.BuscarPorCodigoIntegracao(booking.legBookingList?.FirstOrDefault()?.portDestination?.alternateCodes?.FirstOrDefault()?.code ?? string.Empty);
                Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = repPorto.BuscarPorCodigoIntegracao(booking.legBookingList?.FirstOrDefault()?.portOrigin?.alternateCodes?.FirstOrDefault()?.code ?? string.Empty);

                if (portoDestino == null)
                    portoDestino = repPorto.BuscarPorDescricao(booking.legBookingList?.FirstOrDefault()?.portDestination?.name ?? string.Empty);

                if (portoDestino == null)
                {
                    Servicos.Log.TratarErro("Não foi localizado o porto de destino na interpretação do booking " + (booking?.bookingNumber ?? ""), "Booking");
                    throw new ServicoException("Porto de destino não foi localizado");
                }

                if (portoOrigem == null)
                    portoOrigem = repPorto.BuscarPorDescricao(booking.legBookingList?.FirstOrDefault()?.portOrigin?.name ?? string.Empty);

                if (portoOrigem == null)
                {
                    Servicos.Log.TratarErro("Não foi localizado o porto de origem na interpretação do booking " + (booking?.bookingNumber ?? ""), "Booking");
                    throw new ServicoException("Porto de origem não foi localizado");
                }

                bool dividirCargasPorQuantidadeContainerRecebido = ((bool?)portoDestino.DividirCargasAcordoComQuantidadeContainerRecebidoPortoDestino ?? false) || ((bool?)portoOrigem.DividirCargasAcordoComQuantidadeContainerRecebidoPortoOrigem ?? false);

                if (booking.bookingStatus.Equals("CANCELLED", StringComparison.InvariantCultureIgnoreCase))
                {
                    Servicos.Log.TratarErro("Recebimento de cancelamento do booking " + (booking?.bookingNumber ?? ""), "Booking");
                    List<int> protocolosCargaExistente = repCarga.BuscarProtocolosCargaPorNumeroBooking(booking.bookingNumber);
                    foreach (var protocoloCarga in protocolosCargaExistente)
                    {
                        int protocolo = protocoloCarga;
                        SolicitarCancelamentoCarga(topic, booking, ref protocolo, ref msgRetorno, auditado, tipoServicoMultisoftware);
                    }
                }
                else if (!repCargaPedido.ExistePorNumeroBooking(booking.bookingNumber))
                {
                    Servicos.Log.TratarErro("Não existe carga com o booking " + (booking?.bookingNumber ?? ""), "Booking");

                    if (booking.bookingCustomerType.ToUpper().Equals("CABOTAGE") && dividirCargasPorQuantidadeContainerRecebido)
                        ConverterBookingEmMultiplasCargas(topic, booking, ref msgRetorno, auditado, tipoServicoMultisoftware, ref mensagemErro, ref protocoloCargaExistente, ref protocoloPedidoExistente, servicoPedidoWS, servicoCargaWS, servicoProdutosPedido, servicoCargaIntegracao, configuracaoIntegracaoIntercab, configuracaoIntegracaoEMP);
                    else if ((booking.bookingCustomerType.ToUpper().Equals("CABOTAGE") && !dividirCargasPorQuantidadeContainerRecebido) ||
                        !booking.bookingCustomerType.ToUpper().Equals("CABOTAGE"))
                        ConverterBookingEmCargaIndividual(topic, booking, ref msgRetorno, auditado, tipoServicoMultisoftware, ref mensagemErro, ref protocoloCargaExistente, ref protocoloPedidoExistente, servicoPedidoWS, servicoCargaWS, servicoProdutosPedido, servicoCargaIntegracao, configuracaoIntegracaoIntercab, configuracaoIntegracaoEMP);
                }
                else
                {
                    int cargaPedidos = repCargaPedido.ContarCargasPedidoPorNumeroBooking(booking.bookingNumber);
                    int quantidadePedidosBoking = (int)booking.equipment.quantity;
                    int diferencaEntrePedidos = quantidadePedidosBoking - cargaPedidos;

                    protocoloCargaExistente = repCarga.BuscarProtocoloCargaPorNumeroBooking(booking.bookingNumber);

                    if (diferencaEntrePedidos > 0 && (!dividirCargasPorQuantidadeContainerRecebido || !booking.bookingCustomerType.ToUpper().Equals("CABOTAGE")))
                    {
                        Servicos.Log.TratarErro("Gerando novo pedido ao booking " + (booking?.bookingNumber ?? ""), "Booking");
                        ConverterBookingEmNovoPedido(topic, booking, ref msgRetorno, auditado, tipoServicoMultisoftware, ref mensagemErro, ref protocoloCargaExistente, ref protocoloPedidoExistente, servicoPedidoWS, servicoCargaWS, servicoProdutosPedido, servicoCargaIntegracao, diferencaEntrePedidos, configuracaoIntegracaoIntercab, configuracaoIntegracaoEMP);
                    }
                    else if (diferencaEntrePedidos < 0 && dividirCargasPorQuantidadeContainerRecebido && booking.bookingCustomerType.ToUpper().Equals("CABOTAGE"))
                    {
                        Servicos.Log.TratarErro("Gerando cancelamento de carga excedente " + (booking?.bookingNumber ?? ""), "Booking");
                        VerificarCancelamentoCargaExcedente(topic, booking, ref protocoloCargaExistente, ref msgRetorno, auditado, tipoServicoMultisoftware, diferencaEntrePedidos);
                    }
                    else if (diferencaEntrePedidos > 0 && dividirCargasPorQuantidadeContainerRecebido && booking.bookingCustomerType.ToUpper().Equals("CABOTAGE"))
                    {
                        Servicos.Log.TratarErro("Realizando a conversão de multiplas cargas para o booking " + (booking?.bookingNumber ?? ""), "Booking");
                        ConverterBookingEmMultiplasCargas(topic, booking, ref msgRetorno, auditado, tipoServicoMultisoftware, ref mensagemErro, ref protocoloCargaExistente, ref protocoloPedidoExistente, servicoPedidoWS, servicoCargaWS, servicoProdutosPedido, servicoCargaIntegracao, configuracaoIntegracaoIntercab, configuracaoIntegracaoEMP);
                    }
                    else if (diferencaEntrePedidos < 0 && (!dividirCargasPorQuantidadeContainerRecebido || !booking.bookingCustomerType.ToUpper().Equals("CABOTAGE")))
                    {
                        Servicos.Log.TratarErro("Realizando a remoção de pedido excedente " + (booking?.bookingNumber ?? ""), "Booking");
                        VerificarRemocaoPedidoExcedente(topic, booking, ref protocoloCargaExistente, ref msgRetorno, auditado, tipoServicoMultisoftware);
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Não será processado o booking " + (booking?.bookingNumber ?? ""), "Booking");
                        SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(booking), "Este booking não entrou em nenhuma regra para geração de carga/pedido, favor reveja as configurações e reprocesse o mesmo.", false, TipoIntegracaoEMP.Booking, booking.bookingNumber);
                    }
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro("Erro 1 na interpretação do booking " + (booking?.bookingNumber ?? "") + " " + excecao.Message, "Booking");
                msgRetorno = excecao.Message;
                SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(booking), msgRetorno, false, TipoIntegracaoEMP.Booking, booking.bookingNumber);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("Erro 2 na interpretação do booking " + (booking?.bookingNumber ?? "") + " " + excecao.Message, "Booking");

                Log.TratarErro(excecao);
                msgRetorno = "Problemas ao receber os dados do Booking, verifique a auditoria";

                if (booking != null)
                    SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(booking), msgRetorno, false, TipoIntegracaoEMP.Booking, "NÃO LOCALIZADO");
                else
                    SalvarLogRecebimentoEMP(topic, "", msgRetorno, false, TipoIntegracaoEMP.Booking, "NÃO LOCALIZADO");
            }
        }

        public void RecebimentoContainer(string topic, ContainerViagem containerViagem, out string msgRetorno, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.WebService.Carga.Pedido servicoPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);
            Servicos.WebService.Carga.Carga servicoCargaWS = new Servicos.WebService.Carga.Carga(_unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido servicoProdutosPedido = new WebService.Carga.ProdutosPedido(_unitOfWork);
            Servicos.Embarcador.Carga.CargaIntegracao servicoCargaIntegracao = new Servicos.Embarcador.Carga.CargaIntegracao();

            Repositorio.Embarcador.EMP.ContainerEMP repContainer = new Repositorio.Embarcador.EMP.ContainerEMP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            msgRetorno = "";

            try
            {
                if (containerViagem == null || containerViagem.Containers == null || containerViagem.Containers.Count == 0)
                {
                    msgRetorno = "Container nulo";
                    SalvarLogRecebimentoEMP(topic, "", msgRetorno, false, TipoIntegracaoEMP.Container, "");
                    return;
                }

                foreach (var container in containerViagem.Containers)
                {
                    Dominio.Entidades.Embarcador.EMP.ContainerEMP containerEMP = container.CodigoContainer > 0 ? repContainer.BuscarPorCodigoContainer(container.CodigoContainer) : null;

                    if (containerEMP == null)
                    {
                        containerEMP = new Dominio.Entidades.Embarcador.EMP.ContainerEMP()
                        {
                            CodigoContainer = container.CodigoContainer
                        };
                    }
                    else
                        containerEMP.Initialize();

                    containerEMP.CodigoPaisOrigem = container?.CodigoPaisOrigem ?? 0;
                    containerEMP.CodigoProgramacaoViagemContainer = container?.CodigoProgramacaoViagemContainer ?? 0;
                    containerEMP.CodigoPropContainer = container?.CodigoPropContainer ?? 0;
                    containerEMP.CodigoTipoContainer = container?.CodigoTipoContainer ?? 0;
                    containerEMP.CodigoViagem = container?.CodigoVIagem ?? 0;
                    containerEMP.IndicadorReutilizado = container?.IndicadorReutilizado ?? 0;
                    containerEMP.Lacres = container?.lacres ?? string.Empty;
                    containerEMP.Nome = container?.Nome ?? string.Empty;
                    containerEMP.NumeroCNPJ = container?.NumeroCNPJ ?? string.Empty;
                    containerEMP.NumeroContainer = container?.NumeroContainer ?? string.Empty;
                    containerEMP.NumeroProgramacao = container?.NumeroProgramacao ?? string.Empty;
                    containerEMP.Observacao = container?.Observacao ?? string.Empty;
                    containerEMP.PesoTotal = container?.PesoTotal ?? 0d;
                    containerEMP.Sigla = container?.Sigla ?? string.Empty;
                    containerEMP.ValorTaraEspecifica = container?.ValorTaraEspecifica ?? 0d;
                    containerEMP.Status = StatusContainerEMP.Pendente;

                    if (containerEMP.Codigo > 0)
                        repContainer.Atualizar(containerEMP, auditado);
                    else
                        repContainer.Inserir(containerEMP, auditado);

                    if (repCargaPedido.ExistePorNumeroOS(container?.NumeroProgramacao ?? string.Empty))
                        AtualizarSituacaoCartaEContainer(containerEMP, topic, container, auditado);
                }

                SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(containerViagem), "Recebimento dos dados do Container realizaco com sucesso", true, TipoIntegracaoEMP.Container, "");
            }
            catch (ServicoException excecao)
            {
                msgRetorno = excecao.Message;
                SalvarLogRecebimentoEMP(topic, "", msgRetorno, false, TipoIntegracaoEMP.Container, "");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                msgRetorno = "Problemas ao receber os dados do Container, verifique a auditoria";
                SalvarLogRecebimentoEMP(topic, "", msgRetorno, false, TipoIntegracaoEMP.Container, "");
            }
        }

        public bool RecebimentoNavio(string topic, vesselMessage vessel, out string msgRetorno, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            msgRetorno = "";
            StringBuilder consistencias = new();
            StringBuilder camposObrigatorios = new();

            try
            {
                Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(_unitOfWork);

                if (vessel == null || vessel.vessel == null)
                    consistencias.AppendLine("JSON do Topic Vessel fora do padrão");

                if (string.IsNullOrWhiteSpace(vessel.vessel.vesselID))
                    camposObrigatorios.AppendLine(nameof(vessel.vessel.vesselID));

                if (string.IsNullOrWhiteSpace(vessel.vessel.imoNumber))
                    camposObrigatorios.AppendLine(nameof(vessel.vessel.imoNumber));

                if (string.IsNullOrWhiteSpace(vessel.vessel.vesselName))
                    camposObrigatorios.AppendLine(nameof(vessel.vessel.vesselName));

                if (string.IsNullOrWhiteSpace(vessel.vessel.callSign))
                    camposObrigatorios.AppendLine(nameof(vessel.vessel.callSign));

                if (string.IsNullOrWhiteSpace(vessel.vessel.vesselStatus))
                    camposObrigatorios.AppendLine(nameof(vessel.vessel.vesselStatus));

                if (camposObrigatorios.Length > 0)
                    consistencias.AppendLine($"Campos obrigatórios não informados ({camposObrigatorios.Replace(Environment.NewLine, ",").ToString().TrimEnd(',')})");

                if (consistencias.Length > 0)
                    throw new ServicoException(consistencias.ToString());

                Dominio.Entidades.Embarcador.Pedidos.Navio navio = repNavio.BuscarPorNavioID(vessel.vessel.vesselID);

                if (navio == null && !string.IsNullOrWhiteSpace(vessel.vessel.imoNumber))
                    navio = repNavio.BuscarPorCodigoIMO(vessel.vessel.imoNumber);

                if (navio == null)
                    navio = new Dominio.Entidades.Embarcador.Pedidos.Navio();
                else
                    navio.Initialize();

                navio.NavioID = vessel.vessel.vesselID;
                navio.TipoEmbarcacao = TipoEmbarcacao.Cargueiro;
                if (!string.IsNullOrWhiteSpace(vessel.vessel.vesselName))
                    navio.Descricao = vessel.vessel.vesselName;
                if (!string.IsNullOrWhiteSpace(vessel.vessel.imoNumber))
                    navio.CodigoIMO = vessel.vessel.imoNumber;
                else if (!string.IsNullOrWhiteSpace(vessel.vessel.vesselID))
                    navio.CodigoIMO = vessel.vessel.vesselID;
                if (!string.IsNullOrWhiteSpace(vessel.vessel.vesselID))
                    navio.NavioID = vessel.vessel.vesselID;
                if (!string.IsNullOrWhiteSpace(vessel.vessel.callSign))
                    navio.Irin = vessel.vessel.callSign;
                if (!string.IsNullOrWhiteSpace(vessel.vessel.vesselStatus))
                {
                    if (vessel.vessel.vesselStatus == "Outfleeted")
                        navio.Status = false;
                    else
                        navio.Status = true;
                }

                var operatorDetailActive = vessel.vessel.operatorDetails.FirstOrDefault(o => o.operatorStatus.Equals("Active", StringComparison.OrdinalIgnoreCase));

                if (operatorDetailActive != null)
                {
                    navio.CodigoOperador = operatorDetailActive.operatorCode;
                    navio.CodigoIntegracao = operatorDetailActive.vesselCode;
                }

                if (navio.Codigo > 0)
                    repNavio.Atualizar(navio, auditado);
                else
                    repNavio.Inserir(navio, auditado);

                Repositorio.Embarcador.Pedidos.NavioOperador repNavioOperador = new Repositorio.Embarcador.Pedidos.NavioOperador(_unitOfWork);

                foreach (com.maersk.vessel.smds.operations.MSK.operatorDetail operatorDetail in vessel.vessel.operatorDetails)
                {
                    Dominio.Entidades.Embarcador.Pedidos.NavioOperador navioOperador = repNavioOperador.BuscarPorNavioID(operatorDetail.operatorId, navio.Codigo);

                    if (navioOperador == null)
                    {
                        navioOperador = new Dominio.Entidades.Embarcador.Pedidos.NavioOperador();
                        navioOperador.IdOperador = operatorDetail.operatorId;
                    }

                    navioOperador.CodigoOperador = operatorDetail.operatorCode;
                    navioOperador.CodigoIntegracao = operatorDetail.vesselCode;
                    navioOperador.Navio = navio;
                    navioOperador.DataAtivo = operatorDetail.inFleetDate.GetValueOrDefault() > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(operatorDetail.inFleetDate.Value).UtcDateTime : null;
                    navioOperador.DataInativo = operatorDetail.outFleetDate.GetValueOrDefault() > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(operatorDetail.outFleetDate.Value).UtcDateTime : null;

                    navioOperador.Status = operatorDetail.operatorStatus switch
                    {
                        "Active" => StatusNavioOperador.Ativo,
                        "Inactive" => StatusNavioOperador.Inativo,
                        "Partially Active" => StatusNavioOperador.ParcialmenteAtivo,
                        _ => navioOperador.Status
                    };

                    if (navioOperador.Codigo == 0)
                        repNavioOperador.Inserir(navioOperador);
                    else
                        repNavioOperador.Atualizar(navioOperador);
                }

                msgRetorno = "Sucesso";
                SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(vessel.vessel), msgRetorno, true, TipoIntegracaoEMP.Vessel, "");
                return true;
            }
            catch (ServicoException ex)
            {
                SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(vessel.vessel), ex.Message, false, TipoIntegracaoEMP.Vessel, "");
                return false;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                msgRetorno = "Problemas ao receber os dados do Vessel, verifique a auditoria";
                SalvarLogRecebimentoEMP(topic, "", msgRetorno, false, TipoIntegracaoEMP.Vessel, "");
                return false;
            }
        }

        public bool RecebimentoPessoa(string topic, CustomerMessage customer, out string msgRetorno, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            msgRetorno = "";
            string arquivoRecebido = customer != null && customer.customerEntity != null ? Newtonsoft.Json.JsonConvert.SerializeObject(customer) : "Fora do padrão do AVRO";
            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(_unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);

                if (customer == null || customer.customerEntity == null)
                {
                    msgRetorno = "JSON do Topic Customer fora do padrão";
                    SalvarLogRecebimentoEMP(topic, arquivoRecebido, msgRetorno, false, TipoIntegracaoEMP.Customer, "");
                    return false;
                }

                if ((customer.customerEntity?.customerDetails?.customerCoreInformation?.customerStatus?.statusName ?? "").ToLower() == "suspended")
                {
                    msgRetorno = "Recebimento de customer com status suspended";
                    SalvarLogRecebimentoEMP(topic, arquivoRecebido, msgRetorno, false, TipoIntegracaoEMP.Customer, "", customer.customerEntity?.customerDetails?.customerCoreInformation?.customerCode);
                    return false;
                }

                List<string> camposObrigatorios = new List<string>();

                var fields = new List<Expression<Func<CustomerMessage, object>>>
                {
                    customer => customer.customerEntity.customerDetails.customerCoreInformation,
                    customer => customer.customerEntity.customerDetails.customerCoreInformation.customerCode,
                    customer => customer.customerEntity.customerDetails.customerCoreInformation.telecommunicationNumber.number,
                    customer => customer.customerEntity.customerDetails.customerCoreInformation.customerAddress.streetName,
                    customer => customer.customerEntity.customerDetails.customerCoreInformation.customerAddress.district,
                    customer => customer.customerEntity.customerDetails.customerCoreInformation.customerAddress.cityName,
                    customer => customer.customerEntity.customerDetails.customerCoreInformation.customerAddress.cityCode,
                    customer => customer.customerEntity.customerDetails.customerCoreInformation.customerAddress.region.regionName,
                    customer => customer.customerEntity.customerDetails.customerCoreInformation.customerAddress.postalCode,
                    customer => customer.customerEntity.customerDetails.customerCoreInformation.customerAddress.country.countryName,
                };

                //var firstPath = GetPathAndValue(customer, fields[0]).Path;

                foreach (var item in fields.Skip(1))
                {
                    var result = GetPathAndValue(customer, item);

                    if (result == null || (result is string strValue && string.IsNullOrWhiteSpace(strValue)))
                        camposObrigatorios.Add(item.ToString());
                    //camposObrigatorios.Add(result.Path.Remove(0, firstPath.Length + 1));
                }

                if (string.IsNullOrWhiteSpace(customer.customerEntity?.customerDetails?.customerCoreInformation?.tradingName)
                && string.IsNullOrWhiteSpace(customer.customerEntity?.customerDetails?.customerCoreInformation?.legalName))
                {
                    camposObrigatorios.Add($"{nameof(CustomerMessage.customerEntity.customerDetails.customerCoreInformation.tradingName)} ou {nameof(CustomerMessage.customerEntity.customerDetails.customerCoreInformation.legalName)}");
                }

                var identifierCodes = new[] { "BR1", "BR2", "BR3", "BR4" };
                com.maersk.customer.smds.commercial.msk.CustomerIdentifier identificacao = null;
                if (customer.customerEntity?.customerDetails?.customerIdentifiers != null && customer.customerEntity?.customerDetails?.customerIdentifiers.Count > 0)
                    identificacao = customer.customerEntity?.customerDetails?.customerIdentifiers.Where(c => c.identifierCountry != null && !string.IsNullOrWhiteSpace(c.identifierCountry.isoCountryCode) && c.identifierValue != "0" && (c.identifierCode == "BR1" || c.identifierCode == "BR2"))?.FirstOrDefault();

                if (identificacao != null && identifierCodes.Contains(identificacao.identifierCode) && (string.IsNullOrWhiteSpace(identificacao.identifierValue) || identificacao.identifierValue == "0"))
                {
                    camposObrigatorios.Add($"identifierValue com identifierCode \"{identificacao.identifierCode}\"");
                }

                if (camposObrigatorios.Count > 0)
                {
                    msgRetorno = $"Campos obrigatórios não informados ({string.Join(",", camposObrigatorios)}).\nVerifique o arquivo recebido.";
                    SalvarLogRecebimentoEMP(topic, arquivoRecebido, msgRetorno, false, TipoIntegracaoEMP.Customer, "", customer.customerEntity?.customerDetails?.customerCoreInformation?.customerCode);
                    return false;
                }

                var country = customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.country;

                if ((country == null || country.isoCountryCode != "BR")
                    && (customer.customerEntity?.customerDetails?.customerCoreInformation?.brands?.FirstOrDefault()?.brandCode ?? "") != "ALNN")
                {
                    msgRetorno = "País diferente de Brasil sem a brand ALNN";
                    SalvarLogRecebimentoEMP(topic, arquivoRecebido, msgRetorno, false, TipoIntegracaoEMP.Customer, "", customer.customerEntity?.customerDetails?.customerCoreInformation?.customerCode);
                    return false;
                }

                Dominio.Entidades.Cliente cliente = null;

                bool novoCadastro = false;
                string tipo = "E";
                string cpfcnpj = "";
                double dCPFCNPJ = 0;

                if (identificacao != null && country != null && (country.isoCountryCode == "BR") && !string.IsNullOrWhiteSpace(identificacao?.identifierValue))
                {
                    cpfcnpj = Utilidades.String.OnlyNumbers(identificacao?.identifierValue);
                    if (cpfcnpj.Length == 11)
                        tipo = "F";
                    else
                        tipo = "J";
                    dCPFCNPJ = cpfcnpj.ToDouble();
                }

                string nome = customer.customerEntity?.customerDetails?.customerCoreInformation?.tradingName;
                if (string.IsNullOrWhiteSpace(nome))
                    nome = customer.customerEntity?.customerDetails?.customerCoreInformation?.legalName;

                string codigoIntegracao = customer.customerEntity?.customerDetails?.customerCoreInformation?.customerCode ?? "";
                if (tipo == "E")
                    cliente = !string.IsNullOrWhiteSpace(codigoIntegracao) ? repCliente.BuscarPorCodigoIntegracao(codigoIntegracao) : null;
                else
                    cliente = repCliente.BuscarPorCPFCNPJ(dCPFCNPJ);

                if (cliente == null)
                {
                    cliente = new Dominio.Entidades.Cliente();
                    novoCadastro = true;
                }
                else
                    cliente.Initialize();

                bool pessoaJuridica = identificacao != null && identificacao?.identifierCode == "BR1";
                string inscricaoEstadual = customer.customerEntity?.customerDetails?.customerIdentifiers != null && customer.customerEntity?.customerDetails?.customerIdentifiers.Count > 0 ? customer.customerEntity?.customerDetails?.customerIdentifiers.Where(obj => obj.identifierCode == "BR3")?.FirstOrDefault()?.identifierValue ?? "ISENTO" : "ISENTO";
                string inscricaoMunicipal = customer.customerEntity?.customerDetails?.customerIdentifiers != null && customer.customerEntity?.customerDetails?.customerIdentifiers.Count > 0 ? customer.customerEntity?.customerDetails?.customerIdentifiers.Where(obj => obj.identifierCode == "BR4")?.FirstOrDefault()?.identifierValue ?? "" : "";

                cliente.CodigoIntegracao = codigoIntegracao;
                cliente.IE_RG = inscricaoEstadual;
                cliente.InscricaoMunicipal = inscricaoMunicipal;
                if (novoCadastro)
                {
                    cliente.Tipo = tipo == "E" ? "E" : pessoaJuridica ? "J" : "F";
                    cliente.CPF_CNPJ = tipo == "E" ? repCliente.BuscarPorProximoExterior() : dCPFCNPJ;
                }
                if (!string.IsNullOrWhiteSpace(nome))
                    cliente.Nome = nome;
                if (!string.IsNullOrEmpty(customer.customerEntity?.customerDetails?.customerCoreInformation?.legalName))
                    cliente.NomeFantasia = customer.customerEntity?.customerDetails?.customerCoreInformation?.legalName;
                else if (!string.IsNullOrWhiteSpace(nome))
                    cliente.NomeFantasia = nome;
                if (!string.IsNullOrWhiteSpace(customer.customerEntity?.customerDetails?.customerCoreInformation?.customerStatus?.statusCode))
                    cliente.Ativo = customer.customerEntity?.customerDetails?.customerCoreInformation?.customerStatus?.statusCode == "I" ? false : true;
                if (!string.IsNullOrWhiteSpace(customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.streetName))
                    cliente.Endereco = customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.streetName;
                if (!string.IsNullOrWhiteSpace(customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.district))
                    cliente.Bairro = customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.district;
                if (!string.IsNullOrWhiteSpace(customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.postalCode))
                    cliente.CEP = Utilidades.String.OnlyNumbers(customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.postalCode);
                if (!string.IsNullOrWhiteSpace(customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.streetNumber))
                    cliente.Numero = customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.streetNumber;
                if (!string.IsNullOrWhiteSpace(customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.streetNumber))
                    cliente.Complemento = "";
                if (!string.IsNullOrWhiteSpace(customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.cityCode))
                    cliente.Localidade = repLocalidade.buscarPorCodigoCidadeo(customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.cityCode);
                if (!string.IsNullOrWhiteSpace(customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.latitude))
                    cliente.Latitude = customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.latitude;
                if (!string.IsNullOrWhiteSpace(customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.longitude))
                    cliente.Longitude = customer.customerEntity?.customerDetails?.customerCoreInformation?.customerAddress?.longitude;
                if (!string.IsNullOrWhiteSpace(customer.customerEntity?.customerDetails?.customerCoreInformation?.telecommunicationNumber?.number))
                    cliente.Telefone1 = customer.customerEntity?.customerDetails?.customerCoreInformation?.telecommunicationNumber?.number;

                cliente.TipoLogradouro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Rua;
                cliente.Atividade = repAtividade.BuscarPorCodigo(5);
                cliente.EnderecoDigitado = true;
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;

                if (string.IsNullOrWhiteSpace(cliente.Nome) && !string.IsNullOrWhiteSpace(cliente.NomeFantasia))
                    cliente.Nome = cliente.NomeFantasia;
                if (string.IsNullOrWhiteSpace(cliente.NomeFantasia) && !string.IsNullOrWhiteSpace(cliente.Nome))
                    cliente.NomeFantasia = cliente.Nome;

                if (tipo == "J")
                {
                    string raizCNPJ = "";
                    raizCNPJ = Utilidades.String.OnlyNumbers(cpfcnpj).Remove(8, 6);
                    if (!string.IsNullOrEmpty(raizCNPJ))
                    {
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(raizCNPJ);
                        if (grupoPessoas != null)
                            cliente.GrupoPessoas = grupoPessoas;
                    }
                }

                if (string.IsNullOrWhiteSpace(cliente.Nome) || cliente.Localidade == null)
                {
                    msgRetorno = "Campos obrigatórios não informados (Nome e Localidade).";
                    SalvarLogRecebimentoEMP(topic, arquivoRecebido, msgRetorno, false, TipoIntegracaoEMP.Customer, "", customer.customerEntity?.customerDetails?.customerCoreInformation?.customerCode);
                    return false;
                }

                if (novoCadastro)
                    repCliente.Inserir(cliente, auditado);
                else
                    repCliente.Atualizar(cliente, auditado);

                new Repositorio.Embarcador.Pessoas.PessoaIntegracao(_unitOfWork).GerarIntegracaoPessoa(_unitOfWork, cliente);

                msgRetorno = "Sucesso";
                SalvarLogRecebimentoEMP(topic, arquivoRecebido, msgRetorno, true, TipoIntegracaoEMP.Customer, "", customer.customerEntity?.customerDetails?.customerCoreInformation?.customerCode);
                return true;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                msgRetorno = "Problemas ao receber os dados do Customer, verifique a auditoria";
                SalvarLogRecebimentoEMP(topic, arquivoRecebido, msgRetorno, false, TipoIntegracaoEMP.Customer, "", customer.customerEntity?.customerDetails?.customerCoreInformation?.customerCode);
                return false;
            }
        }

        public bool RecebimentoSchedule(string topic, ScheduleEvent schedule, out string msgRetorno, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            msgRetorno = "";
            string arquivoRecebido = schedule != null ? Newtonsoft.Json.JsonConvert.SerializeObject(schedule) : "JSON do Topic schedule fora do padrão";
            string viagemNavioDirecao = null;

            try
            {
                ValidarCamposRecebimentoSchedule(schedule);

                _unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(_unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repPedidoViagemNavioSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(_unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio = new Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio();

                Dominio.Entidades.Embarcador.Pedidos.Navio navio = repNavio.BuscarPorCodigoIMO(schedule.vessel.imoNumber) ?? throw new ServicoException($"Navio com IMO '{schedule.vessel.imoNumber}' não localizado");
                DirecaoViagemMultimodal direcao = DirecaoViagemMultimodalHelper.ConverterDoIngles(schedule.voyage.direction?.ToUpper());
                viagemNavioDirecao = BuildViagemNavioDescricao(schedule);
                pedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigoImo(schedule.vessel.imoNumber, direcao, int.Parse(schedule.voyage.voyageNumber));

                if (pedidoViagemNavio?.Status == false)
                {
                    pedidoViagemNavio = null;
                }
                else if (pedidoViagemNavio != null && schedule.header.transactionType == com.schedule.dto.TransactionType.CREATED)
                {
                    throw new ServicoException($"Viagem {viagemNavioDirecao} já existe");
                }

                if (pedidoViagemNavio == null && (schedule.header.transactionType == com.schedule.dto.TransactionType.UPDATED || schedule.header.transactionType == com.schedule.dto.TransactionType.DELETED))
                {
                    throw new ServicoException($"Viagem {viagemNavioDirecao} não encontrada");
                }

                bool inserindo = false;

                if (schedule.header.transactionType == com.schedule.dto.TransactionType.CREATED || schedule.header.transactionType == com.schedule.dto.TransactionType.UPDATED)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule> pedidoViagemNavioSchedules = new();

                    if (pedidoViagemNavio == null)
                    {
                        pedidoViagemNavio = new Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio();
                        inserindo = true;
                    }
                    else
                    {
                        pedidoViagemNavio.Initialize();
                    }

                    pedidoViagemNavio.Integrado = true;
                    pedidoViagemNavio.Status = true;
                    pedidoViagemNavio.Descricao = viagemNavioDirecao;
                    pedidoViagemNavio.CodigoIntegracao = schedule.scheduleID;
                    pedidoViagemNavio.NumeroViagem = int.Parse(schedule.voyage.voyageNumber);
                    pedidoViagemNavio.DirecaoViagemMultimodal = direcao;
                    pedidoViagemNavio.Navio = navio;

                    if (pedidoViagemNavio.Codigo == 0)
                        repPedidoViagemNavio.Inserir(pedidoViagemNavio, auditado);
                    else
                        repPedidoViagemNavio.Atualizar(pedidoViagemNavio, auditado);

                    List<(int codigoTerminal, int codigoPorto)> codigosPortoETerminal = new();

                    foreach (var portCall in schedule.portCalls)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule pedidoViagemNavioSchedule = new Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule();

                        string codigoTerminalAtracacao = portCall.terminalAlternativeCodes.Where(t => t.alternativeCodeType.Contains("MaerskCode")).FirstOrDefault().alternativeCode;
                        string codigoPortoAtracacao = portCall.portAlternativeCodes.Where(t => t.alternativeCodeType.Equals("portCode") || t.alternativeCodeType.Equals("PortCode")).FirstOrDefault().alternativeCode;

                        Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = repTipoTerminalImportacao.BuscarPorCodigoIntegracao(codigoTerminalAtracacao);

                        if (terminal == null)
                            throw new ServicoException($"Terminal com código {codigoTerminalAtracacao} na sequência {portCall.sequence} não encontrado");

                        Dominio.Entidades.Embarcador.Pedidos.Porto porto = repPorto.BuscarPorCodigoMercante(codigoPortoAtracacao);

                        if (porto == null)
                            throw new ServicoException($"Porto com código {codigoPortoAtracacao} na sequência {portCall.sequence} não encontrado");

                        codigosPortoETerminal.Add((terminal.Codigo, porto.Codigo));

                        if (schedule.header.transactionType == com.schedule.dto.TransactionType.UPDATED)
                        {
                            pedidoViagemNavioSchedule = repPedidoViagemNavioSchedule.BuscarPorViagemPortoTerminal(pedidoViagemNavio.Codigo, porto.Codigo, terminal.Codigo);
                            if (pedidoViagemNavioSchedule != null)
                                pedidoViagemNavioSchedule.Initialize();
                            else
                                pedidoViagemNavioSchedule = new Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule();
                        }

                        pedidoViagemNavioSchedule.PedidoViagemNavio = pedidoViagemNavio;

                        pedidoViagemNavioSchedule.TerminalAtracacao = terminal;
                        pedidoViagemNavioSchedule.PortoAtracacao = porto;

                        DateTime.TryParse(portCall.cargoDeadline, out DateTime dataDeadLineSchedule);
                        DateTime.TryParse(portCall.estimatedTimeOfDeparture, out DateTime dataPrevisaoSaidaSchedule);
                        DateTime.TryParse(portCall.estimatedTimeOfArrival, out DateTime dataPrevisaoChegadaSchedule);
                        DateTime.TryParse(portCall.actualTimeOfDeparture, out DateTime dataETSSchedule);
                        DateTime.TryParse(portCall.actualTimeOfArrival, out DateTime dataETASchedule);

                        pedidoViagemNavioSchedule.Status = true;
                        pedidoViagemNavioSchedule.DataPrevisaoChegadaNavio = dataPrevisaoChegadaSchedule > DateTime.MinValue ? dataPrevisaoChegadaSchedule : null;
                        pedidoViagemNavioSchedule.DataPrevisaoSaidaNavio = dataPrevisaoSaidaSchedule > DateTime.MinValue ? dataPrevisaoSaidaSchedule : null;
                        pedidoViagemNavioSchedule.DataDeadLine = dataDeadLineSchedule > DateTime.MinValue ? dataDeadLineSchedule : null;
                        pedidoViagemNavioSchedule.ETAConfirmado = dataETASchedule > DateTime.MinValue;
                        pedidoViagemNavioSchedule.ETSConfirmado = dataETSSchedule > DateTime.MinValue;

                        if (pedidoViagemNavioSchedule.Codigo == 0)
                            repPedidoViagemNavioSchedule.Inserir(pedidoViagemNavioSchedule, auditado);
                        else
                            repPedidoViagemNavioSchedule.Atualizar(pedidoViagemNavioSchedule, auditado);
                    }

                    if (schedule.header.transactionType == com.schedule.dto.TransactionType.UPDATED && !inserindo)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule> schedules = repPedidoViagemNavioSchedule.BuscarPorPedidoViagemNavio(pedidoViagemNavio.Codigo);
                        foreach (var pvnSchedule in schedules)
                        {
                            //Exclui os schedules não encontrados na lista dos portCalls enviados na atualização
                            if (!codigosPortoETerminal.Any(c => pvnSchedule.TerminalAtracacao.Codigo == c.codigoTerminal && pvnSchedule.PortoAtracacao.Codigo == c.codigoPorto))
                            {
                                repPedidoViagemNavioSchedule.Deletar(pvnSchedule, auditado);
                            }
                        }
                    }
                }

                if (schedule.header.transactionType == com.schedule.dto.TransactionType.DELETED && !inserindo)
                {
                    Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote repEnvioDocumentacaoLote = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote(_unitOfWork);
                    Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(_unitOfWork);
                    Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.ConsultarPorPedidoViagemNavio(pedidoViagemNavio.Codigo);

                    if (pedidos?.Any() == true)
                    {
                        string numeroBooking = pedidos
                            .Select(p => p.NumeroBooking)
                            .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));

                        if (numeroBooking != null)
                            throw new ServicoException($"Não foi possível deletar o schedule, pois ele já foi utilizado no booking {numeroBooking})");
                    }
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> pedidosTransbordo = repPedido.ConsultarPedidoTransbordoPorPedidoViagemNavio(pedidoViagemNavio.Codigo);
                        if (pedidosTransbordo?.Any() == true)
                        {
                            string numeroBooking = pedidosTransbordo
                                .Select(p => p.Pedido.NumeroBooking)
                                .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));

                            if (numeroBooking != null)
                                throw new ServicoException($"Não foi possível deletar o schedule, pois ele já foi utilizado no booking {numeroBooking})");
                        }
                    }

                    if (repFatura.ExisteFaturaPorViagem(pedidoViagemNavio.Codigo))
                        throw new ServicoException($"Não foi possível deletar o schedule, pois ele já foi utilizado em uma fatura");

                    if (repFaturamentoLote.ExisteFaturamentoLotePorViagem(pedidoViagemNavio.Codigo))
                        throw new ServicoException($"Não foi possível deletar o schedule, pois ele já foi utilizado em um fluxo de faturamento automático ou em lote");

                    if (repEnvioDocumentacaoLote.ExisteDocumentacaoLotePorViagem(pedidoViagemNavio.Codigo))
                        throw new ServicoException($"Não foi possível deletar o schedule, pois ele já foi utilizado em um fluxo de documentação automático ou em lote");

                    repPedidoViagemNavio.Deletar(pedidoViagemNavio, auditado);
                }
                else if (pedidoViagemNavio != null && !inserindo)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, pedidoViagemNavio, null, "VVD Atualizado via integração do EMP.", _unitOfWork);


                _unitOfWork.CommitChanges();

                msgRetorno = "Sucesso";
                SalvarLogRecebimentoEMP(topic, arquivoRecebido, msgRetorno, true, TipoIntegracaoEMP.Schedule, scheduleViagemNavio: BuildViagemNavioDescricao(schedule));
                return true;
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao.Message, nameof(schedule));
                msgRetorno = excecao.Message;
                SalvarLogRecebimentoEMP(topic, arquivoRecebido, msgRetorno, false, TipoIntegracaoEMP.Schedule, scheduleViagemNavio: BuildViagemNavioDescricao(schedule));
                return false;
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();

                Log.TratarErro(excecao);
                msgRetorno = "Problemas ao receber os dados do schedule, verifique a auditoria";
                SalvarLogRecebimentoEMP(topic, arquivoRecebido, msgRetorno, false, TipoIntegracaoEMP.Schedule, scheduleViagemNavio: BuildViagemNavioDescricao(schedule));
                return false;
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados - Estrutura Padrão

        private void HandlerOcorrencia(DeliveryReport<Null, string> deliveryReport)
        {
            _deliveryHandler = deliveryReport;

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void HandlerCCe(DeliveryReport<Null, string> deliveryReport)
        {
            _deliveryHandler = deliveryReport;

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void HandlerCarga(DeliveryReport<Null, string> deliveryReport)
        {
            _deliveryHandler = deliveryReport;

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void HandlerCTe(DeliveryReport<Null, string> deliveryReport)
        {
            _deliveryHandler = deliveryReport;

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void HandlerCTeManual(DeliveryReport<Null, string> deliveryReport)
        {
            _deliveryHandler = deliveryReport;

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void HandlerFatura(DeliveryReport<Null, string> deliveryReport)
        {
            _deliveryHandler = deliveryReport;

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogErroEMP(string msgErro, string topic, string jsonEnvio, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogEMP(DeliveryResult<Null, string> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogEMP(DeliveryResult<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogEMP(DeliveryResult<string, EMPCTeNormal> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogEMP(DeliveryResult<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogEMP(DeliveryResult<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogEMP(DeliveryResult<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogEMP(DeliveryResult<string, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogEMP(DeliveryResult<string, anl.documentation.ctetransport.TransportDocumentationCTE> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogEMP(DeliveryResult<string, anl.documentation.CCetransport.correctionLetter> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogEMP(DeliveryResult<string, com.alianca.logisticsoperations.emp.transportplan.transportplan> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogEMP(DeliveryResult<string, Dominio.ObjetosDeValor.WebService.CTe.CTeFatura> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

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

        private void SalvarLogEMP<T>(DeliveryResult<string, T> deliveryReport, TipoIntegracaoEMP tipoIntegracao, string numeroBooking)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, nameof(Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog));

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

        private void ValidarConfiguracaoIntegracao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP)
        {
            if (configuracaoIntegracaoEMP == null || !configuracaoIntegracaoEMP.PossuiIntegracaoEMP || string.IsNullOrWhiteSpace(configuracaoIntegracaoEMP.UsuarioEMP) || string.IsNullOrWhiteSpace(configuracaoIntegracaoEMP.SenhaEMP))
                throw new ServicoException("É necessário definir as configurações de integração EMP.");
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

        private void EfetuarIntegracaoEMPRetina(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP, string topic, object objetoJson, out string arquivoEnvio, out string arquivoRetorno, TipoIntegracaoEMP tipoIntegracao, string url)
        {
            arquivoEnvio = "";
            arquivoRetorno = "";

            var jsonSerializerConfig = new JsonSerializerConfig
            {
                BufferBytes = 999999
            };

            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = configuracaoIntegracaoEMP.BootstrapServerRetina,//"emp-kafka-shared-ppwe-retina.maersk-digital.net:443",
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.ScramSha512,
                SslCaLocation = (configuracaoIntegracaoEMP.CertificadoCRTServerRetina?.NomeArquivo ?? ""),//"C:\\Empresas\\Certificado\\ca.crt",//crt//"C:\\Empresas\\Certificado\\navega\\ca.crt"
                SaslUsername = configuracaoIntegracaoEMP.UsuarioServerRetina,//"PIJWNGSIBAQ5FKT4",
                SaslPassword = configuracaoIntegracaoEMP.SenhaServerRetina//"YzBlMTViYmVlMzliNTU3ZmE5NjkyMTBj"
            };

            if (configuracaoIntegracaoEMP.AtivarEnvioSerializaçãoEMP && (objetoJson.GetType() == typeof(anl.documentation.ctetransport.TransportDocumentationCTE) || objetoJson.GetType().BaseType == typeof(anl.documentation.ctetransport.TransportDocumentationCTE)))
            {
                var headers = new Headers();
                headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
                Message<string, anl.documentation.ctetransport.TransportDocumentationCTE> kafkaMessage = new Message<string, anl.documentation.ctetransport.TransportDocumentationCTE>()
                {
                    Key = "TransportDocumentationCTE",
                    Value = (anl.documentation.ctetransport.TransportDocumentationCTE)objetoJson,
                    Headers = headers
                };

                arquivoEnvio = JsonConvert.SerializeObject(kafkaMessage.Value);

                using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                    new SchemaRegistryConfig
                    {
                        Url = configuracaoIntegracaoEMP.URLSchemaRegistryRetina, //"https://emp-pp-sr.retina.maersk-digital.net",
                        SslKeystorePassword = configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina,//"nxs9a08DskoOpas2d34lwEs",
                        SslKeystoreLocation = (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "")//"C:\\Empresas\\Certificado\\certificadoP12SchemaRegistryRetina.p12"
                    }))
                using (IProducer<string, anl.documentation.ctetransport.TransportDocumentationCTE> producer = new ProducerBuilder<string, anl.documentation.ctetransport.TransportDocumentationCTE>(config)
                     .SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                     .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<anl.documentation.ctetransport.TransportDocumentationCTE>(schemaRegistry).AsSyncOverAsync())
                     .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
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
            else if (configuracaoIntegracaoEMP.AtivarEnvioSerializaçãoEMP && (objetoJson.GetType() == typeof(anl.documentation.CCetransport.correctionLetter) || objetoJson.GetType().BaseType == typeof(anl.documentation.CCetransport.correctionLetter)))
            {
                var headers = new Headers();
                headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
                Message<string, anl.documentation.CCetransport.correctionLetter> kafkaMessage = new Message<string, anl.documentation.CCetransport.correctionLetter>()
                {
                    //Key = "correctionLetter",
                    Value = (anl.documentation.CCetransport.correctionLetter)objetoJson,
                    Headers = headers
                };

                arquivoEnvio = JsonConvert.SerializeObject(kafkaMessage.Value);

                using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                    new SchemaRegistryConfig
                    {
                        Url = configuracaoIntegracaoEMP.URLSchemaRegistryRetina, //"https://emp-pp-sr.retina.maersk-digital.net",
                        SslKeystorePassword = configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina,//"nxs9a08DskoOpas2d34lwEs",
                        SslKeystoreLocation = (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "")//"C:\\Empresas\\Certificado\\certificadoP12SchemaRegistryRetina.p12"
                    }))
                using (IProducer<string, anl.documentation.CCetransport.correctionLetter> producer = new ProducerBuilder<string, anl.documentation.CCetransport.correctionLetter>(config)
                     //.SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                     .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<anl.documentation.CCetransport.correctionLetter>(schemaRegistry).AsSyncOverAsync())
                     .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
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
            else if (configuracaoIntegracaoEMP.AtivarEnvioSerializaçãoEMP && (objetoJson.GetType() == typeof(com.alianca.logisticsoperations.emp.transportplan.transportplan) || objetoJson.GetType().BaseType == typeof(com.alianca.logisticsoperations.emp.transportplan.transportplan)))
            {
                var headers = new Headers();
                headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
                Message<string, com.alianca.logisticsoperations.emp.transportplan.transportplan> kafkaMessage = new Message<string, com.alianca.logisticsoperations.emp.transportplan.transportplan>()
                {
                    //Key = "correctionLetter",
                    Value = (com.alianca.logisticsoperations.emp.transportplan.transportplan)objetoJson,
                    Headers = headers
                };

                arquivoEnvio = JsonConvert.SerializeObject(kafkaMessage.Value);

                using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                    new SchemaRegistryConfig
                    {
                        Url = configuracaoIntegracaoEMP.URLSchemaRegistryRetina, //"https://emp-pp-sr.retina.maersk-digital.net",
                        SslKeystorePassword = configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina,//"nxs9a08DskoOpas2d34lwEs",
                        SslKeystoreLocation = (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "")//"C:\\Empresas\\Certificado\\certificadoP12SchemaRegistryRetina.p12"
                    }))
                using (IProducer<string, com.alianca.logisticsoperations.emp.transportplan.transportplan> producer = new ProducerBuilder<string, com.alianca.logisticsoperations.emp.transportplan.transportplan>(config)
                     //.SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                     .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<com.alianca.logisticsoperations.emp.transportplan.transportplan>(schemaRegistry).AsSyncOverAsync())
                     .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
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
            else if (configuracaoIntegracaoEMP.AtivarEnvioSerializaçãoEMP && (objetoJson.GetType() == typeof(Dominio.ObjetosDeValor.WebService.CTe.CTeFatura) || objetoJson.GetType().BaseType == typeof(Dominio.ObjetosDeValor.WebService.CTe.CTeFatura)))
            {
                var headers = new Headers();
                headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
                Message<string, Dominio.ObjetosDeValor.WebService.CTe.CTeFatura> kafkaMessage = new Message<string, Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>()
                {
                    Value = (Dominio.ObjetosDeValor.WebService.CTe.CTeFatura)objetoJson,
                    Headers = headers
                };

                arquivoEnvio = JsonConvert.SerializeObject(kafkaMessage.Value);

                using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                    new SchemaRegistryConfig
                    {
                        Url = configuracaoIntegracaoEMP.URLSchemaRegistryRetina, //"https://emp-pp-sr.retina.maersk-digital.net",
                        SslKeystorePassword = configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina,//"nxs9a08DskoOpas2d34lwEs",
                        SslKeystoreLocation = (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "")//"C:\\Empresas\\Certificado\\certificadoP12SchemaRegistryRetina.p12"
                    }))
                using (IProducer<string, Dominio.ObjetosDeValor.WebService.CTe.CTeFatura> producer = new ProducerBuilder<string, Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>(config)
                     //.SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                     .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>(schemaRegistry).AsSyncOverAsync())
                     .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
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
            else if (configuracaoIntegracaoEMP.AtivarEnvioSerializaçãoEMP && (objetoJson.GetType() == typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal) || objetoJson.GetType().BaseType == typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal)))
            {
                var headers = new Headers();
                headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
                Message<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal> kafkaMessage = new Message<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal>()
                {
                    Value = (Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal)objetoJson,
                    Headers = headers
                };

                JsonSerializer _jsonWriter = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                arquivoEnvio = JsonConvert.SerializeObject(kafkaMessage.Value, Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

                using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                    new SchemaRegistryConfig
                    {
                        Url = configuracaoIntegracaoEMP.URLSchemaRegistryRetina, //"https://emp-pp-sr.retina.maersk-digital.net",
                        SslKeystorePassword = configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina,//"nxs9a08DskoOpas2d34lwEs",
                        SslKeystoreLocation = (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "")//"C:\\Empresas\\Certificado\\certificadoP12SchemaRegistryRetina.p12"//"C:\\Empresas\\Certificado\\navega\\ca.p12",//
                    }))
                using (IProducer<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal> producer = new ProducerBuilder<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal>(config)
                     //.SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                     .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal>(schemaRegistry).AsSyncOverAsync())
                     .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
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
            else if (configuracaoIntegracaoEMP.AtivarEnvioSerializaçãoEMP && (objetoJson.GetType() == typeof(EMPCTeNormal) || objetoJson.GetType().BaseType == typeof(EMPCTeNormal)))
            {
                var headers = new Headers();
                headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
                Message<string, EMPCTeNormal> kafkaMessage = new Message<string, EMPCTeNormal>()
                {
                    Value = (EMPCTeNormal)objetoJson,
                    Headers = headers
                };
                JsonSerializer _jsonWriter = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                arquivoEnvio = JsonConvert.SerializeObject(kafkaMessage.Value, Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

                using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                    new SchemaRegistryConfig
                    {
                        Url = configuracaoIntegracaoEMP.URLSchemaRegistryRetina, //"https://emp-pp-sr.retina.maersk-digital.net",
                        SslKeystorePassword = configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina,//"nxs9a08DskoOpas2d34lwEs",
                        SslKeystoreLocation = (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "")//"C:\\Empresas\\Certificado\\certificadoP12SchemaRegistryRetina.p12"//"C:\\Empresas\\Certificado\\navega\\ca.p12",//
                    }))
                using (IProducer<string, EMPCTeNormal> producer = new ProducerBuilder<string, EMPCTeNormal>(config)
                     //.SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                     .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<EMPCTeNormal>(schemaRegistry).AsSyncOverAsync())
                     .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
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
            else if (configuracaoIntegracaoEMP.AtivarEnvioSerializaçãoEMP && (objetoJson.GetType() == typeof(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao) || objetoJson.GetType().BaseType == typeof(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao)))
            {
                var headers = new Headers();
                headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
                Message<string, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> kafkaMessage = new Message<string, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>()
                {
                    Value = (Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao)objetoJson,
                    Headers = headers
                };

                arquivoEnvio = JsonConvert.SerializeObject(kafkaMessage.Value);

                using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                    new SchemaRegistryConfig
                    {
                        Url = configuracaoIntegracaoEMP.URLSchemaRegistryRetina, //"https://emp-pp-sr.retina.maersk-digital.net",
                        SslKeystorePassword = configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina,//"nxs9a08DskoOpas2d34lwEs",
                        SslKeystoreLocation = (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "")//"C:\\Empresas\\Certificado\\certificadoP12SchemaRegistryRetina.p12"
                    }))
                using (IProducer<string, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> producer = new ProducerBuilder<string, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>(config)
                     //.SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                     .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>(schemaRegistry).AsSyncOverAsync())
                     .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
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
            else if (configuracaoIntegracaoEMP.AtivarEnvioSerializaçãoEMP && (objetoJson.GetType() == typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar) || objetoJson.GetType().BaseType == typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar)))
            {
                var headers = new Headers();
                headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
                Message<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar> kafkaMessage = new Message<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar>()
                {
                    Value = (Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar)objetoJson,
                    Headers = headers
                };

                arquivoEnvio = JsonConvert.SerializeObject(kafkaMessage.Value);

                using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                    new SchemaRegistryConfig
                    {
                        Url = configuracaoIntegracaoEMP.URLSchemaRegistryRetina, //"https://emp-pp-sr.retina.maersk-digital.net",
                        SslKeystorePassword = configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina,//"nxs9a08DskoOpas2d34lwEs",
                        SslKeystoreLocation = (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "")//"C:\\Empresas\\Certificado\\certificadoP12SchemaRegistryRetina.p12"
                    }))
                using (IProducer<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar> producer = new ProducerBuilder<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar>(config)
                     //.SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                     .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar>(schemaRegistry).AsSyncOverAsync())
                     .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
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
            else if (configuracaoIntegracaoEMP.AtivarEnvioSerializaçãoEMP && (objetoJson.GetType() == typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento) || objetoJson.GetType().BaseType == typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento)))
            {
                var headers = new Headers();
                headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
                Message<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento> kafkaMessage = new Message<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento>()
                {
                    Value = (Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento)objetoJson,
                    Headers = headers
                };

                arquivoEnvio = JsonConvert.SerializeObject(kafkaMessage.Value);

                using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                    new SchemaRegistryConfig
                    {
                        Url = configuracaoIntegracaoEMP.URLSchemaRegistryRetina, //"https://emp-pp-sr.retina.maersk-digital.net",
                        SslKeystorePassword = configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina,//"nxs9a08DskoOpas2d34lwEs",
                        SslKeystoreLocation = (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "")//"C:\\Empresas\\Certificado\\certificadoP12SchemaRegistryRetina.p12"
                    }))
                using (IProducer<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento> producer = new ProducerBuilder<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento>(config)
                     //.SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                     .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento>(schemaRegistry).AsSyncOverAsync())
                     .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
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
            else if (configuracaoIntegracaoEMP.AtivarEnvioSerializaçãoEMP && (objetoJson.GetType() == typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual) || objetoJson.GetType().BaseType == typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual)))
            {
                var headers = new Headers();
                headers.Add("Http-Connection", Encoding.ASCII.GetBytes(url));
                Message<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual> kafkaMessage = new Message<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual>()
                {
                    Value = (Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual)objetoJson,
                    Headers = headers
                };

                arquivoEnvio = JsonConvert.SerializeObject(kafkaMessage.Value);

                using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                    new SchemaRegistryConfig
                    {
                        Url = configuracaoIntegracaoEMP.URLSchemaRegistryRetina, //"https://emp-pp-sr.retina.maersk-digital.net",
                        SslKeystorePassword = configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina,//"nxs9a08DskoOpas2d34lwEs",
                        SslKeystoreLocation = (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "")//"C:\\Empresas\\Certificado\\certificadoP12SchemaRegistryRetina.p12"
                    }))
                using (IProducer<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual> producer = new ProducerBuilder<string, Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual>(config)
                     //.SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                     .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual>(schemaRegistry).AsSyncOverAsync())
                     .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
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
            else if (configuracaoIntegracaoEMP.AtivarEnvioSerializaçãoEMP && (objetoJson.GetType() == typeof(anl.documentation.CeMerchantNumber.CeMerchant) || objetoJson.GetType().BaseType == typeof(anl.documentation.CeMerchantNumber.CeMerchant)))
            {
                var headers = new Headers
                {
                    { "Http-Connection", Encoding.ASCII.GetBytes(url) }
                };
                Message<string, anl.documentation.CeMerchantNumber.CeMerchant> kafkaMessage = new Message<string, anl.documentation.CeMerchantNumber.CeMerchant>()
                {
                    Key = nameof(anl.documentation.CeMerchantNumber.CeMerchant),
                    Value = (anl.documentation.CeMerchantNumber.CeMerchant)objetoJson,
                    Headers = headers
                };

                arquivoEnvio = JsonConvert.SerializeObject(kafkaMessage.Value);

                using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                    new SchemaRegistryConfig
                    {
                        Url = configuracaoIntegracaoEMP.URLSchemaRegistryRetina, //"https://emp-pp-sr.retina.maersk-digital.net",
                        SslKeystorePassword = configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina,//"nxs9a08DskoOpas2d34lwEs",
                        SslKeystoreLocation = (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "")//"C:\\Empresas\\Certificado\\certificadoP12SchemaRegistryRetina.p12"
                    }))
                using (IProducer<string, anl.documentation.CeMerchantNumber.CeMerchant> producer = new ProducerBuilder<string, anl.documentation.CeMerchantNumber.CeMerchant>(config)
                     .SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                     .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<anl.documentation.CeMerchantNumber.CeMerchant>(schemaRegistry).AsSyncOverAsync())
                     .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
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

        private void SalvarLogRecebimentoEMP(string topic, string arquivoJson, string msgRetorno, bool sucesso, TipoIntegracaoEMP tipoIntegracao, string numeroBooking = null, string customerCode = null, string scheduleViagemNavio = null)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEMPLogRecebimento repIntegracaoEMPLogRecebimento = new Repositorio.Embarcador.Integracao.IntegracaoEMPLogRecebimento(_unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento()
            {
                ArquivoRecebimento = arquivoJson, //fazer salvar em arquivo??
                DataRecebimento = DateTime.Now,
                MensageRetorno = msgRetorno,
                Sucesso = sucesso,
                Topic = topic,
                Justificativa = msgRetorno,
                SituacaoIntegracao = sucesso ? SituacaoIntegracaoEMP.Integrado : SituacaoIntegracaoEMP.NotPersist,
                TipoIntegracao = tipoIntegracao,
                NumeroBooking = numeroBooking,
                CustomerCode = customerCode,
                ScheduleViagemNavio = scheduleViagemNavio
            };
            repIntegracaoEMPLogRecebimento.Inserir(integracaoEMPLog);
        }

        #endregion Métodos Privados - Estrutura Padrão

        #region Métodos Privados

        private void ConverterBookingEmMultiplasCargas(string topic, IntercabDocBooking booking, ref string msgRetorno, Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref StringBuilder mensagemErro, ref int protocoloCargaExistente, ref int protocoloPedidoExistente, WebService.Carga.Pedido servicoPedidoWS, WebService.Carga.Carga servicoCargaWS, ProdutosPedido servicoProdutosPedido, Servicos.Embarcador.Carga.CargaIntegracao servicoCargaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab configuracaoIntegracaoIntercab, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = servicoCargaIntegracao.ConverterBookingEmCargaIntegracao(booking, configuracaoIntegracaoIntercab?.TipoOperacao, _unitOfWork, configuracaoIntegracaoEMP);

            string numeroCarga = cargaIntegracao.NumeroCarga;

            for (int i = 1; i <= cargaIntegracao.QuantidadeContainerBooking; i++)
            {
                int numeracao = 1;

                cargaIntegracao.NumeroCarga = numeroCarga + "-" + i.ToString();

                while (repCargaPedido.ExisteMesmoNumeroBooking(cargaIntegracao.NumeroCarga))
                {
                    numeracao++;
                    cargaIntegracao.NumeroCarga = numeroCarga + "-" + numeracao.ToString();
                };

                if (numeracao > cargaIntegracao.QuantidadeContainerBooking)
                    break;

                GerarCargaBooking(topic, booking, ref msgRetorno, auditado, tipoServicoMultisoftware, ref mensagemErro, ref protocoloCargaExistente, ref protocoloPedidoExistente, servicoPedidoWS, servicoCargaWS, servicoProdutosPedido, cargaIntegracao);
            }

            GerarLogRecebimentoGeracaoCarga(topic, booking, msgRetorno, mensagemErro);
        }

        private void ConverterBookingEmCargaIndividual(string topic, IntercabDocBooking booking, ref string msgRetorno, Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref StringBuilder mensagemErro, ref int protocoloCargaExistente, ref int protocoloPedidoExistente, WebService.Carga.Pedido servicoPedidoWS, WebService.Carga.Carga servicoCargaWS, ProdutosPedido servicoProdutosPedido, Servicos.Embarcador.Carga.CargaIntegracao servicoCargaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab configuracaoIntegracaoIntercab, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP)
        {
            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = servicoCargaIntegracao.ConverterBookingEmCargaIntegracao(booking, configuracaoIntegracaoIntercab?.TipoOperacao, _unitOfWork, configuracaoIntegracaoEMP);

            for (int i = 1; i <= cargaIntegracao.QuantidadeContainerBooking; i++)
            {
                if (i == cargaIntegracao.QuantidadeContainerBooking)
                    cargaIntegracao.FecharCargaAutomaticamente = true;
                else
                    cargaIntegracao.FecharCargaAutomaticamente = false;

                GerarCargaBooking(topic, booking, ref msgRetorno, auditado, tipoServicoMultisoftware, ref mensagemErro, ref protocoloCargaExistente, ref protocoloPedidoExistente, servicoPedidoWS, servicoCargaWS, servicoProdutosPedido, cargaIntegracao);
            }

            GerarLogRecebimentoGeracaoCarga(topic, booking, msgRetorno, mensagemErro);
        }

        private void GerarLogRecebimentoGeracaoCarga(string topic, IntercabDocBooking booking, string msgRetorno, StringBuilder mensagemErro)
        {
            if (mensagemErro.Length == 0)
            {
                msgRetorno = "Dados do Booking recebidos e carga criada com sucesso";
                SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(booking), msgRetorno, true, TipoIntegracaoEMP.Booking, booking.bookingNumber);
            }
            else
                SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(booking), mensagemErro.ToString(), false, TipoIntegracaoEMP.Booking, booking.bookingNumber);
        }

        private void ConverterBookingEmNovoPedido(string topic, IntercabDocBooking booking, ref string msgRetorno, Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref StringBuilder mensagemErro, ref int protocoloCargaExistente, ref int protocoloPedidoExistente, WebService.Carga.Pedido servicoPedidoWS, WebService.Carga.Carga servicoCargaWS, ProdutosPedido servicoProdutosPedido, Servicos.Embarcador.Carga.CargaIntegracao servicoCargaIntegracao, int diferencaEntrePedidos, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab configuracaoIntegracaoIntercab, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP)
        {
            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = servicoCargaIntegracao.ConverterBookingEmCargaIntegracao(booking, configuracaoIntegracaoIntercab?.TipoOperacao, _unitOfWork, configuracaoIntegracaoEMP);

            for (int i = 1; i <= diferencaEntrePedidos; i++)
                AdicionarPedidoCarga(topic, booking, ref msgRetorno, auditado, tipoServicoMultisoftware, ref mensagemErro, ref protocoloCargaExistente, ref protocoloPedidoExistente, servicoPedidoWS, servicoCargaWS, servicoProdutosPedido, cargaIntegracao);
        }

        private void GerarCargaBooking(string topic, IntercabDocBooking booking, ref string msgRetorno, Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref StringBuilder mensagemErro, ref int protocoloCargaExistente, ref int protocoloPedidoExistente, WebService.Carga.Pedido servicoPedidoWS, WebService.Carga.Carga servicoCargaWS, ProdutosPedido servicoProdutosPedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab configuracaoIntegracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = servicoPedidoWS.CriarPedido(cargaIntegracao, null, configuracaoIntegracaoIntercab?.TipoOperacao, ref mensagemErro, tipoServicoMultisoftware, ref protocoloPedidoExistente, ref protocoloCargaExistente, false, auditado, configuracaoTMS, null, "", ignorarPedidosInseridosManualmente: true, false);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

            pedido.PossuiCargaPerigosa = cargaIntegracao.ContemCargaPerigosa;
            pedido.ImprimirObservacaoCTe = cargaIntegracao.ImprimirObservacaoCTe;

            if (mensagemErro.Length == 0)
            {
                servicoProdutosPedido.AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref mensagemErro, _unitOfWork, auditado);
                if (cargaIntegracao.Transbordo != null && cargaIntegracao.Transbordo.Count > 0)
                    servicoProdutosPedido.SalvarTransbordo(pedido, cargaIntegracao.Transbordo, ref mensagemErro, _unitOfWork, _unitOfWork.StringConexao, auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);

                cargaPedido = servicoCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref mensagemErro, ref protocoloCargaExistente, _unitOfWork, tipoServicoMultisoftware, false, false, auditado, configuracaoTMS, new AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso(), "", null, configuracaoIntegracaoIntercab?.TipoOperacao);
                if (cargaPedido != null)
                    servicoCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemErro, _unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);

                if (mensagemErro.Length == 0 && cargaPedido.Carga != null)
                {
                    if (cargaIntegracao.FecharCargaAutomaticamente)
                        SolicitarFechamentoCarga(auditado, tipoServicoMultisoftware, configuracaoTMS, cargaPedido);
                }
                else if (mensagemErro.Length == 0 && cargaPedido.Carga == null)
                {
                    mensagemErro.Append("Carga não criada, favor verifique os dados recebidos.");
                }
            }
        }

        private void AdicionarPedidoCarga(string topic, IntercabDocBooking booking, ref string msgRetorno, Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref StringBuilder mensagemErro, ref int protocoloCargaExistente, ref int protocoloPedidoExistente, WebService.Carga.Pedido servicoPedidoWS, WebService.Carga.Carga servicoCargaWS, ProdutosPedido servicoProdutosPedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            int codigoCargaExistente = 0;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repCargaPedido.BuscarPorProtocoloCarga(protocoloCargaExistente);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = cargasPedidos.FirstOrDefault().Carga;
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab configuracaoIntegracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            if (cargasPedidos == null || cargasPedidos.Count == 0)
                throw new ServicoException($"Não foi encontrado nenhuma carga/pedido com o protocolo informado. {protocoloCargaExistente}");

            if (cargaOrigem.SituacaoCarga != SituacaoCarga.AgNFe && cargaOrigem.SituacaoCarga != SituacaoCarga.Nova)
                throw new ServicoException($"A atual situação da carga {cargaOrigem.CodigoCargaEmbarcador} não permite adicionar novos pedidos;");

            if (cargaOrigem.ProcessandoDocumentosFiscais)
                throw new ServicoException("A atual situação da carga (Processando Documentos Fiscais) não permite que ela seja modificada; ");

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Servicos.WebService.Carga.Pedido(_unitOfWork).CriarPedido(cargaIntegracao, null, configuracaoIntegracaoIntercab?.TipoOperacao, ref mensagemErro, tipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, false, auditado, configuracaoTMS);

            servicoProdutosPedido.AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref mensagemErro, _unitOfWork, auditado);
            if (cargaIntegracao.Transbordo != null && cargaIntegracao.Transbordo.Count > 0)
                servicoProdutosPedido.SalvarTransbordo(pedido, cargaIntegracao.Transbordo, ref mensagemErro, _unitOfWork, _unitOfWork.StringConexao, auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);

            cargaPedido = servicoCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref mensagemErro, ref protocoloCargaExistente, _unitOfWork, tipoServicoMultisoftware, false, false, auditado, configuracaoTMS, new AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso(), "", null, configuracaoIntegracaoIntercab?.TipoOperacao);
            if (cargaPedido != null)
                servicoCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemErro, _unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);

            if (cargaOrigem.CargaAgrupamento != null)
            {
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupamento = cargaOrigem.CargaAgrupamento;
                serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref cargaAgrupamento, configuracaoTMS, _unitOfWork, tipoServicoMultisoftware);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCarga = repCargaPedido.BuscarPedidosPorCarga(cargaOrigem.Codigo);
            foreach (var pedidoCarga in pedidosCarga)
            {
                pedidoCarga.QuantidadeContainerBooking = pedidoCarga.QuantidadeContainerBooking + 1;
                repPedido.Atualizar(pedidoCarga);
            }

            if (mensagemErro.Length == 0)
            {
                msgRetorno = "Dados do Booking recebidos e pedido adicionado com sucesso";
                SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(booking), msgRetorno, true, TipoIntegracaoEMP.Booking, booking.bookingNumber);
            }
            else
            {
                SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(booking), mensagemErro.ToString(), false, TipoIntegracaoEMP.Booking, booking.bookingNumber);
            }
        }

        private void SolicitarFechamentoCarga(Auditado auditado, TipoServicoMultisoftware tipoServicoMultisoftware, ConfiguracaoTMS configuracaoTMS, CargaPedido cargaPedido)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            if (configuracaoTMS.AgruparCargaAutomaticamente)
                cargaPedido.Carga.AgruparCargaAutomaticamente = true;
            else if (!configuracaoTMS.FecharCargaPorThread)
            {
                new Servicos.Embarcador.Carga.Carga(_unitOfWork).FecharCarga(cargaPedido.Carga, _unitOfWork, tipoServicoMultisoftware, null);

                cargaPedido.Carga.CargaFechada = true;

                new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).Atualizar(cargaPedido.Carga);
            }
            else
                cargaPedido.Carga.FechandoCarga = true;

            repCarga.Atualizar(cargaPedido.Carga);
            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, "Solicitou fechamento da carga por Integração Booking. Protocolo " + cargaPedido.Carga.Codigo.ToString(), _unitOfWork);
        }

        private void VerificarCancelamentoCargaExcedente(string topic, IntercabDocBooking booking, ref int protocoloCargaExistente, ref string msgRetorno, Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int cargasExcedentes)
        {
            int quantidadeCargasExcedentes = Math.Abs(cargasExcedentes);
            int canceladas = 0;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repCargaPedido.BuscarCargaPedidosPorNumeroBookingOrdenadosDesc(booking.bookingNumber);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidos)
            {
                if (quantidadeCargasExcedentes == canceladas)
                    break;

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaPedido.Carga.Codigo);
                protocoloCargaExistente = carga.Protocolo;

                string mensagemErro = ValidarDadosCancelamentoCarga(carga, configuracaoTMS, ref protocoloCargaExistente);
                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    continue;

                SolicitarCancelamentoCarga(topic, booking, ref protocoloCargaExistente, ref msgRetorno, auditado, tipoServicoMultisoftware);
                canceladas++;
            }

            if (canceladas == 0)
                SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(booking), "Não foi possível cancelar nenhuma das cargas. Verifique o arquivo de integração.", false, TipoIntegracaoEMP.Booking, booking.bookingNumber);
        }

        private void VerificarRemocaoPedidoExcedente(string topic, IntercabDocBooking booking, ref int protocoloCargaExistente, ref string msgRetorno, Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool removido = false;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repCargaPedido.BuscarCargaPedidosPorNumeroBookingOrdenadosDesc(booking.bookingNumber);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidos)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

                if (cargaPedido.Pedido.Container != null)
                    continue;

                try
                {
                    Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoVinculadoCarga(cargaPedido, _unitOfWork, configuracaoTMS, tipoServicoMultisoftware, new AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente(), removerPedido: false);
                }
                catch (ServicoException)
                {
                    continue;
                }

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCarga = repCargaPedido.BuscarPedidosPorCarga(carga.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoCarga in pedidosCarga)
                {
                    pedidoCarga.QuantidadeContainerBooking = pedidoCarga.QuantidadeContainerBooking - 1;

                    if (pedidoCarga.QuantidadeContainerBooking < 0)
                        pedidoCarga.QuantidadeContainerBooking = 0;

                    repositorioPedido.Atualizar(pedidoCarga);
                }

                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, "Excluiu pedido vinculado por Integração Booking.", _unitOfWork);
                removido = true;
            }

            if (removido)
                SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(booking), "Pedido foi removido através da integração do booking", false, TipoIntegracaoEMP.Booking, booking.bookingNumber);
            else
                SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(booking), "Não foi possível remover nenhum pedido. Verifique o arquivo de integração.", false, TipoIntegracaoEMP.Booking, booking.bookingNumber);
        }

        private void SolicitarCancelamentoCarga(string topic, IntercabDocBooking booking, ref int protocoloCargaExistente, ref string msgRetorno, Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Servicos.Embarcador.Carga.CargaCancelamentoAprovacao servicoCargaCancelamentoAprovacao = new Servicos.Embarcador.Carga.CargaCancelamentoAprovacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocoloCargaExistente);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga?.Codigo ?? 0);

            string mensagemErro = ValidarDadosCancelamentoCarga(carga, configuracaoTMS, ref protocoloCargaExistente);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                throw new ServicoException(mensagemErro);

            _unitOfWork.Start();

            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
            {
                Carga = carga,
                DefinirSituacaoEmCancelamento = true,
                DuplicarCarga = false,
                MotivoCancelamento = "Cancelamento solicitado através da integração do booking",
                TipoServicoMultisoftware = tipoServicoMultisoftware,
                GerarIntegracoes = true
            };

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoTMS, _unitOfWork);
            servicoCargaCancelamentoAprovacao.CriarAprovacao(cargaCancelamento, tipoServicoMultisoftware);

            if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.AgAprovacaoSolicitacao)
            {
                msgRetorno = $"Carga: {carga.CodigoCargaEmbarcador} está aguardando aprovação para cancelamento";
                repositorioCargaCancelamento.Atualizar(cargaCancelamento);
            }
            else
            {
                Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, _unitOfWork, _unitOfWork.StringConexao, tipoServicoMultisoftware, false);

                if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                    msgRetorno = $"{cargaCancelamento.MensagemRejeicaoCancelamento} Codigo da Carga: {carga.Codigo} Codigo Carga Embarcador: {carga.CodigoCargaEmbarcador}";
                else if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.Cancelada)
                    msgRetorno = $"Carga: {carga.CodigoCargaEmbarcador} foi cancelada";
                else
                    msgRetorno = $"Carga: {carga.CodigoCargaEmbarcador} está em cancelamento"; ;
            }

            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Cancelamento da carga solicitado por integração do booking.", _unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Pedido, "Cancelamento do pedido solicitado por integração do booking.", _unitOfWork);

            _unitOfWork.CommitChanges();

            SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(booking), msgRetorno, true, TipoIntegracaoEMP.Booking, booking.bookingNumber);
        }

        private string ValidarDadosCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, ref int protocoloCargaExistente)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            string erro;

            if (carga == null)
                return $"Protocolo da carga {protocoloCargaExistente} não encontrado.";

            erro = serCarga.ValidarSeCargaEstaAptaParaCancelamento(carga, "Cancelamento solicitado através da integração do booking", _unitOfWork);
            if (!string.IsNullOrWhiteSpace(erro))
                return erro;

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                return "Já foi solicitado o cancelamento da carga.";

            if ((configuracaoWebService?.NaoPermitirSolicitarCancelamentoCargaViaIntegracaoViagemIniciada ?? false) && carga.DataInicioViagem.HasValue)
                return "Ambiente configurado para não permitir cancelar cargas já iniciadas via integração.";

            return erro;
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

        private List<anl.documentation.CCetransport.correctionLetterInfo> ConverterObjetoCamposCartaCorrecaoRetina(Dominio.Entidades.CartaDeCorrecaoEletronica cartaCorrecao)
        {
            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(_unitOfWork);

            List<Dominio.Entidades.ItemCCe> itensCCe = repItemCCe.BuscarPorCCe(cartaCorrecao.Codigo);

            List<anl.documentation.CCetransport.correctionLetterInfo> camposCCe = new List<anl.documentation.CCetransport.correctionLetterInfo>();

            foreach (Dominio.Entidades.ItemCCe item in itensCCe)
            {
                Dominio.Entidades.CampoCCe campoCCe = item.CampoAlterado;

                anl.documentation.CCetransport.correctionLetterInfo objetoCampoCce = new anl.documentation.CCetransport.correctionLetterInfo()
                {
                    fieldName = campoCCe.NomeCampo,
                    currentValue = "não informado",  //não temos essa informação
                    newValue = item.ValorAlterado
                };

                camposCCe.Add(objetoCampoCce);
            }

            return camposCCe;
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

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento> ConverterRetornoCTeCancelamento(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes)
        {
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento> CTesIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento>();

            if (listaCTes.Count > 0)
            {
                foreach (var cte in listaCTes)
                    CTesIntegracao.Add(serCTe.ConverterObjetoCTeCancelamento(cte, new List<Dominio.Entidades.CTeContaContabilContabilizacao>(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, _unitOfWork, false));
            }

            return CTesIntegracao;

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

        private List<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura> ConverterRetornoCTes(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes, Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
            List<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura> ctesEnvio = new List<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>();

            if (listaCTes.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCtes = listaCTes.SelectMany(o => o.CargaCTes).ToList();

                foreach (var cargaCTe in cargasCtes)
                    ctesEnvio.Add(serCTe.ConverterObjetoCargaCTeFatura(cargaCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, _unitOfWork, false, fatura));
            }

            return ctesEnvio;
        }

        private com.alianca.mtms.emp.billinginvoice.Invoice ConverterInvoice(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string numeroBooking)
        {
            com.alianca.mtms.emp.billinginvoice.Invoice invoice = new com.alianca.mtms.emp.billinginvoice.Invoice
            {
                issueDateTime = faturaIntegracao?.Fatura?.DataFatura.ToDateString() ?? string.Empty,
                invoiceNumber = faturaIntegracao?.Fatura?.Numero.ToString() ?? string.Empty,
                recipient = new com.alianca.mtms.emp.billinginvoice.Recipient
                {
                    legalName = faturaIntegracao?.Fatura?.Cliente?.Nome ?? string.Empty,
                    customerTax = faturaIntegracao?.Fatura?.Cliente?.CPF_CNPJ_SemFormato ?? string.Empty,
                    customerIE = faturaIntegracao?.Fatura?.Cliente?.IE_RG ?? string.Empty,
                    streetNumber = faturaIntegracao?.Fatura?.Cliente?.Numero ?? string.Empty,
                    streetName = (faturaIntegracao?.Fatura?.Cliente?.Endereco ?? string.Empty) + ", " + (faturaIntegracao?.Fatura?.Cliente?.Complemento ?? string.Empty),
                    district = faturaIntegracao?.Fatura?.Cliente?.Bairro ?? string.Empty,
                    subdivisionName = faturaIntegracao?.Fatura?.Cliente?.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                    postalCode = faturaIntegracao?.Fatura?.Cliente?.CEP ?? string.Empty,
                },
                receivingBank = new com.alianca.mtms.emp.billinginvoice.ReceivingBank
                {
                    legalName = faturaIntegracao?.Fatura?.Banco?.Descricao ?? string.Empty,
                    agency = faturaIntegracao?.Fatura?.Agencia ?? string.Empty,
                    accountNumber = faturaIntegracao?.Fatura?.NumeroConta ?? string.Empty,
                    bankCode = faturaIntegracao?.Fatura?.Banco?.Numero.ToString() ?? string.Empty
                },
                totalInvoiceValue = Convert.ToDouble(faturaIntegracao?.Fatura?.Total ?? 0),
                valueInWords = Utilidades.Conversor.DecimalToWords(faturaIntegracao?.Fatura?.Total ?? 0),
                documents = new List<com.alianca.mtms.emp.billinginvoice.Document>
                            {
                                new com.alianca.mtms.emp.billinginvoice.Document
                                {
                                    value = Convert.ToDouble(faturaIntegracao?.Fatura?.Total ?? 0),
                                    itemQuantity = faturaIntegracao?.Fatura?.Parcelas?.Count ?? 0,
                                    arrivalDatetime = faturaIntegracao?.Fatura?.DataFinal?.ToDateString() ?? string.Empty,
                                    cteVessel = new List<com.alianca.mtms.emp.billinginvoice.CTEVessel>
                                    {
                                        new com.alianca.mtms.emp.billinginvoice.CTEVessel
                                        {
                                            vesselName = faturaIntegracao?.Fatura?.PedidoViagemNavio?.Navio?.Descricao ?? string.Empty,
                                            voyageNumber = faturaIntegracao?.Fatura?.PedidoViagemNavio?.NumeroViagem.ToString() ?? string.Empty,
                                            direction = faturaIntegracao?.Fatura?.PedidoViagemNavio?.DirecaoViagemMultimodal.ConverterParaOutroIdioma() ?? string.Empty
                                        }
                                    },
                                    bookingNumber = faturaIntegracao?.Fatura?.Documentos?.Select(doc => doc.Documento?.CTe?.NumeroBooking).FirstOrDefault(numero => !string.IsNullOrEmpty(numero)) ?? string.Empty,
                                    documentNumber = faturaIntegracao?.Fatura?.Documentos?.Select(doc => doc.Documento?.CTe?.Numero.ToString()).ToList() ?? new List<string>()
                                }
                            },
                @operator = faturaIntegracao?.Fatura?.Usuario?.Nome ?? string.Empty,
                status = faturaIntegracao?.Fatura?.DataCancelamentoFatura.HasValue == true ? "Cancelada" : "Gerada"
            };

            return invoice;
        }

        private BillingDocument ConverterBillingDocument(Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao cartaCorrecaoIntegracao, string numeroBooking)
        {
            BillingDocument billingDocument = new()
            {
                BillingDocumentDetails = new BillingDocumentDetails
                {
                    Status = cartaCorrecaoIntegracao?.CartaCorrecao?.Status.ToString() ?? string.Empty,
                    Shipper = new Shipper
                    {
                        LegalName = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Empresa?.RazaoSocial ?? string.Empty,
                        CustomerCode = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Empresa?.CNPJ ?? string.Empty,
                    },
                    Recipient = new Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.AVRO.Alianca.Recipient
                    {
                        LegalName = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Empresa?.RazaoSocial ?? string.Empty,
                        CustomerCode = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Empresa?.CNPJ ?? string.Empty,
                        PostalAddressCombined = new PostalAddress
                        {
                            StreetNumber = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Fatura?.Cliente?.Numero ?? string.Empty,
                            StreetName = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Fatura?.Cliente?.Endereco ?? string.Empty,
                            UnitNumber = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Fatura?.Cliente?.Complemento ?? string.Empty,
                            District = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Fatura?.Cliente?.Bairro ?? string.Empty,
                        }
                    },
                    PaymentDetails = new PaymentDetails
                    {
                        PaymentDate = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Fatura?.DataFinal?.ToDateString() ?? string.Empty,
                        AgencyAssignorCode = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Fatura?.Agencia ?? string.Empty,
                        OurNumber = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Titulo?.NossoNumero ?? string.Empty,
                        TotalInvoiceValue = Convert.ToDouble(cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Fatura?.Total ?? 0),
                        IssueDateTime = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Fatura?.DataFatura.ToDateString() ?? string.Empty,
                        DocumentNumber = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Empresa?.Numero ?? string.Empty,
                        DocType = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Descricao ?? string.Empty,
                        AcceptanceCode = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Descricao ?? string.Empty,
                        ProcessingDate = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.DataIntegracao?.ToDateString() ?? string.Empty,
                        Wallet = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Descricao ?? string.Empty,
                        CurrencyType = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.ValorTotalMercadoria.ToString() ?? string.Empty,
                    },
                    Instructions = new Instructions
                    {
                        DocumentNumber = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Empresa?.Numero ?? string.Empty,
                        Booking = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.NumeroBooking ?? string.Empty,
                        ProtestDays = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Numero ?? 0,
                        InterestPerDelayedDay = Convert.ToDouble(cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Empresa?.Numero ?? "0"),
                        FineForDelay = Convert.ToDouble(cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Empresa?.Numero ?? "0"),
                    },

                    BarCode = cartaCorrecaoIntegracao?.CartaCorrecao?.CTe?.Titulo?.CodigoBarrasBoleto ?? string.Empty
                }
            };

            return billingDocument;
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

        private List<anl.documentation.CCetransport.cteInfo> PreencherCTes(CartaDeCorrecaoEletronica cartaCorrecao)
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

        private List<anl.documentation.CCetransport.correctionLetterInfo> PreencherCorrectionLetterInfo(CartaDeCorrecaoEletronica cartaCorrecao)
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

        private void AtualizarSituacaoCartaEContainer(Dominio.Entidades.Embarcador.EMP.ContainerEMP containerEMP, string topic, object objetoJson, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.EMP.ContainerEMP repContainerEMP = new Repositorio.Embarcador.EMP.ContainerEMP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorNumeroOSComFetch(containerEMP.NumeroProgramacao);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

            Dominio.Entidades.Embarcador.Pedidos.Container container = repContainer.BuscarPorNumero(containerEMP.NumeroContainer);
            if (container == null)
            {
                container = new Dominio.Entidades.Embarcador.Pedidos.Container();
                container.CodigoIntegracao = containerEMP.CodigoContainer.ToString();
                container.Descricao = containerEMP.NumeroContainer;
                container.Numero = Utilidades.String.SanitizeString(containerEMP.NumeroContainer);
                container.Status = true;

                repContainer.Inserir(container);
            }

            pedido.Initialize();
            pedido.Container = container;
            pedido.TaraContainer = containerEMP.ValorTaraEspecifica.ToString();
            if (!string.IsNullOrWhiteSpace(containerEMP.Lacres))
                pedido.LacreContainerUm = containerEMP.Lacres;

            containerEMP.Initialize();
            containerEMP.Carga = cargaPedido.Carga;
            containerEMP.Status = StatusContainerEMP.Finalizado;

            repPedido.Atualizar(pedido, auditado);
            repContainerEMP.Atualizar(containerEMP, auditado);

            string msgRetorno = "Container recebido e informações do pedido atualizadas";
            SalvarLogRecebimentoEMP(topic, Newtonsoft.Json.JsonConvert.SerializeObject(objetoJson), msgRetorno, true, TipoIntegracaoEMP.Container, "");
        }

        private void ValidarCamposRecebimentoBooking(IntercabDocBooking booking)
        {
            StringBuilder consistencias = new StringBuilder();
            List<string> camposObrigatorios = new List<string>();

            if (booking == null)
                throw new ServicoException("JSON do Topic Booking fora do padrão");

            if (!string.IsNullOrWhiteSpace(booking.bookingStatus))
            {
                if (!new string[] { "CONFIRMED", "CANCELLED" }.Contains(booking.bookingStatus, StringComparer.InvariantCultureIgnoreCase))
                {
                    throw new ServicoException("Informação de \"bookingStatus\" diferente de Confirmed e Cancelled");
                }
            }

            var campos = new List<(string Path, object Value)>
            {
                GetPathAndValue(() => booking.bookingNumber),
                GetPathAndValue(() => booking.bookingStatus),
                GetPathAndValue(() => booking.bookingCustomerType),
                GetPathAndValue(() => booking.bookingResponsibleForPayment),
                GetPathAndValue(() => booking.agreementNumber),
            };

            if (booking.agreementCorporateGroup == null)
                campos.Add(GetPathAndValue(() => booking.agreementCorporateGroup));
            else
                campos.Add(GetPathAndValue(() => booking.agreementCorporateGroup.corporateGroupName));

            //Remetente
            if (booking.agreementCustomer == null)
            {
                campos.Add(GetPathAndValue(() => booking.agreementCustomer));
            }
            else
            {
                campos.Add(GetPathAndValue(() => booking.agreementCustomer.customerName));
                campos.Add(GetPathAndValue(() => booking.agreementCustomer.taxID));
                campos.Add(GetPathAndValue(() => booking.agreementCustomer.stateRegistration));
                campos.Add(GetPathAndValue(() => booking.agreementCustomer.zipCode));
                campos.Add(GetPathAndValue(() => booking.agreementCustomer.address));
                campos.Add(GetPathAndValue(() => booking.agreementCustomer.number));
                campos.Add(GetPathAndValue(() => booking.agreementCustomer.district));
                if (booking.agreementCustomer.city == null)
                    campos.Add(GetPathAndValue(() => booking.agreementCustomer.city));
                else
                    campos.Add(GetPathAndValue(() => booking.agreementCustomer.city.name));
                if (booking.agreementCustomer.state == null)
                    campos.Add(GetPathAndValue(() => booking.agreementCustomer.state));
                else
                    campos.Add(GetPathAndValue(() => booking.agreementCustomer.state.name));
            };

            //Destinatário
            if (booking.bookingShipper == null)
                campos.Add(GetPathAndValue(() => booking.bookingShipper));
            else
            {
                campos.Add(GetPathAndValue(() => booking.bookingShipper.customerName));
                campos.Add(GetPathAndValue(() => booking.bookingShipper.taxID));
                campos.Add(GetPathAndValue(() => booking.bookingShipper.stateRegistration));
                campos.Add(GetPathAndValue(() => booking.bookingShipper.zipCode));
                campos.Add(GetPathAndValue(() => booking.bookingShipper.address));
                campos.Add(GetPathAndValue(() => booking.bookingShipper.number));
                campos.Add(GetPathAndValue(() => booking.bookingShipper.district));
                if (booking.bookingShipper.city == null)
                    campos.Add(GetPathAndValue(() => booking.bookingShipper.city));
                else
                    campos.Add(GetPathAndValue(() => booking.bookingShipper.city.name));

                if (booking.bookingShipper.state == null)
                    campos.Add(GetPathAndValue(() => booking.bookingShipper.state));
                else
                    campos.Add(GetPathAndValue(() => booking.bookingShipper.state.name));
            };

            if (booking.legBookingList == null || booking.legBookingList.Count == 0)
            {
                campos.Add(GetPathAndValue(() => booking.legBookingList));
            }
            else
            {
                for (int i = 0; i < booking.legBookingList.Count; i++)
                {
                    if (booking.legBookingList[i] == null)
                    {
                        campos.Add(GetPathAndValue(() => booking.legBookingList[i], i));
                        continue;
                    }

                    //Vessel/VVD/Navio
                    campos.Add(GetPathAndValue(() => booking.legBookingList[i].vesselImoNumber, i));
                    campos.Add(GetPathAndValue(() => booking.legBookingList[i].callSign, i));

                    if (booking.legBookingList[i].vesselName == null)
                        campos.Add(GetPathAndValue(() => booking.legBookingList[i].vesselName, i));
                    else
                    {
                        campos.Add(GetPathAndValue(() => booking.legBookingList[i].vesselName.name, i));

                        if (booking.legBookingList[i].vesselName.alternateCodes == null || booking.legBookingList[i].vesselName.alternateCodes.Count == 0)
                            campos.Add(GetPathAndValue(() => booking.legBookingList[i].vesselName.alternateCodes, i));
                        else
                        {
                            for (int j = 0; j < booking.legBookingList[i].vesselName.alternateCodes.Count; j++)
                            {
                                if (booking.legBookingList[i].vesselName.alternateCodes[j] == null)
                                    campos.Add(GetPathAndValue(() => booking.legBookingList[i].vesselName.alternateCodes[j], i, j));
                                else
                                    campos.Add(GetPathAndValue(() => booking.legBookingList[i].vesselName.alternateCodes[j].code, i, j));
                            }
                        }
                    }

                    campos.Add(GetPathAndValue(() => booking.legBookingList[i].vesselShortName, i));
                    campos.Add(GetPathAndValue(() => booking.legBookingList[i].voyageAndDirection, i));

                    //Porto Origem
                    if (booking.legBookingList[i].portOrigin == null)
                        campos.Add(GetPathAndValue(() => booking.legBookingList[i].portOrigin, i));
                    else
                        campos.Add(GetPathAndValue(() => booking.legBookingList[i].portOrigin.name, i));
                    //Terminal Origem
                    if (booking.legBookingList[i].portTerminalOrigin == null)
                        campos.Add(GetPathAndValue(() => booking.legBookingList[i].portTerminalOrigin, i));
                    else
                        campos.Add(GetPathAndValue(() => booking.legBookingList[i].portTerminalOrigin.name, i));
                    //Porto Destino
                    if (booking.legBookingList[i].portDestination == null)
                        campos.Add(GetPathAndValue(() => booking.legBookingList[i].portDestination, i));
                    else
                        campos.Add(GetPathAndValue(() => booking.legBookingList[i].portDestination.name, i));
                    //Terminal Destino
                    if (booking.legBookingList[i].portTerminalDestination == null)
                        campos.Add(GetPathAndValue(() => booking.legBookingList[i].portTerminalDestination, i));
                    else
                        campos.Add(GetPathAndValue(() => booking.legBookingList[i].portTerminalDestination.name, i));
                }
            };

            if (booking.equipment == null)
                campos.Add(GetPathAndValue(() => booking.equipment));
            else
            {
                campos.Add(GetPathAndValue(() => booking.equipment.quantity));
                campos.Add(GetPathAndValue(() => booking.equipment.size));
                campos.Add(GetPathAndValue(() => booking.equipment.type));
                campos.Add(GetPathAndValue(() => booking.equipment.equipmentTareWeight));
            }

            if (booking.pricingArchitectureEvent == null)
                campos.Add(GetPathAndValue(() => booking.pricingArchitectureEvent));
            else
                campos.Add(GetPathAndValue(() => booking.pricingArchitectureEvent.totalRate));

            if (booking.commodity == null)
                campos.Add(GetPathAndValue(() => booking.commodity));
            else
                campos.Add(GetPathAndValue(() => booking.commodity.name));

            foreach (var campo in campos)
            {
                if (campo.Value == null || (campo.Value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    camposObrigatorios.Add(campo.Path);
                }
            }

            if (camposObrigatorios.Any())
                consistencias.AppendLine($"Campos obrigatórios não informados ({string.Join(",", camposObrigatorios)})");

            if (consistencias.Length > 0)
            {
                Servicos.Log.TratarErro(consistencias.ToString(), "Booking");
                throw new ServicoException(consistencias.ToString());
            }
        }

        private void ValidarCamposRecebimentoSchedule(com.schedule.dto.ScheduleEvent schedule)
        {
            StringBuilder consistencias = new StringBuilder();
            List<string> camposObrigatorios = new List<string>();

            if (schedule == null)
                throw new ServicoException("JSON do Topic schedule fora do padrão");

            var campos = new List<(string Path, object Value)>
            {
                GetPathAndValue(() => schedule.header.transactionType),
                GetPathAndValue(() => schedule.voyage.voyageNumber),
                GetPathAndValue(() => schedule.voyage.direction),
                GetPathAndValue(() => schedule.vessel.vesselName),
                GetPathAndValue(() => schedule.vessel.imoNumber),
                GetPathAndValue(() => schedule.vessel.callSign),
                GetPathAndValue(() => schedule.vessel.operatorCode),
            };

            if (schedule.header.transactionType != TransactionType.DELETED)
            {
                if (schedule.portCalls == null || schedule.portCalls.Count == 0)
                {
                    campos.Add(GetPathAndValue(() => schedule.portCalls));
                }
                else
                {
                    for (int i = 0; i < schedule.portCalls.Count; i++)
                    {
                        if (schedule.portCalls[i] == null)
                        {
                            campos.Add(GetPathAndValue(() => schedule.portCalls[i], i));
                            continue;
                        }

                        campos.Add(GetPathAndValue(() => schedule.portCalls[i].sequence, i));
                        campos.Add(GetPathAndValue(() => schedule.portCalls[i].portName, i));

                        var alternativeCodeTypeToSearch = "portCode";
                        var portAlternativeCode = schedule.portCalls[i].portAlternativeCodes.FirstOrDefault(pac => pac.alternativeCodeType.Equals("portCode") || pac.alternativeCodeType.Equals("PortCode"));

                        var portAlternativeCodeIndex = Reflection.FindIndexInCollection(schedule.portCalls[i].portAlternativeCodes, pac => pac.alternativeCodeType.Equals("portCode") || pac.alternativeCodeType.Equals("PortCode"));

                        if (portAlternativeCode is null)
                            camposObrigatorios.Add(GetPathAndValue(() => schedule.portCalls[i].portAlternativeCodes[0].alternativeCodeType, i).Path + $"='{alternativeCodeTypeToSearch}'");
                        else
                        {
                            if (portAlternativeCode.alternativeCode == null || (portAlternativeCode.alternativeCode is string str && string.IsNullOrWhiteSpace(str)))
                                camposObrigatorios.Add(GetPathAndValue(() => schedule.portCalls[i].portAlternativeCodes[portAlternativeCodeIndex].alternativeCode, i).Path + $"='{alternativeCodeTypeToSearch}'");
                        }

                        alternativeCodeTypeToSearch = "MaerskCode";
                        var terminalAlternativeCode = schedule.portCalls[i].terminalAlternativeCodes
                            .FirstOrDefault(pac => pac.alternativeCodeType.Equals("MaerskCode") || pac.alternativeCodeType.Equals("maerskCode"));

                        var terminalAlternativeCodeIndex = Reflection.FindIndexInCollection(schedule.portCalls[i].terminalAlternativeCodes, pac => pac.alternativeCodeType.Equals("MaerskCode") || pac.alternativeCodeType.Equals("maerskCode"));

                        if (terminalAlternativeCode is null)
                            camposObrigatorios.Add(GetPathAndValue(() => schedule.portCalls[i].terminalAlternativeCodes[0].alternativeCodeType, i).Path + $"='{alternativeCodeTypeToSearch}'");
                        else
                        {
                            if (terminalAlternativeCode.alternativeCode == null || (terminalAlternativeCode.alternativeCode is string str && string.IsNullOrWhiteSpace(str)))
                                camposObrigatorios.Add(GetPathAndValue(() => schedule.portCalls[i].terminalAlternativeCodes[terminalAlternativeCodeIndex].alternativeCode, i).Path + $"='{alternativeCodeTypeToSearch}'");
                        }
                    }
                };
            }

            foreach (var campo in campos)
            {
                if (campo.Value == null || (campo.Value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    camposObrigatorios.Add(campo.Path);
                }
            }

            if (camposObrigatorios.Any())
                consistencias.AppendLine($"Campos obrigatórios não informados ({string.Join(",", camposObrigatorios)})");

            if (!int.TryParse(schedule.voyage.voyageNumber, out int _))
                consistencias.AppendLine($"Campo {GetPathAndValue(() => schedule.voyage.voyageNumber).Path} deve ser um número");

            if (consistencias.Length > 0)
            {
                throw new ServicoException(consistencias.ToString());
            }
        }

        private string BuildViagemNavioDescricao(ScheduleEvent schedule)
        {
            return $"{schedule.vessel.vesselName}/{schedule.voyage.voyageNumber}{DirecaoViagemMultimodalHelper.ObterAbreviacao(DirecaoViagemMultimodalHelper.ConverterDoIngles(schedule.voyage.direction?.ToUpper()))}";
        }

        #endregion Métodos Privados
    }
}
