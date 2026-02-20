using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Servicos.Embarcador.Integracao.Intercab
{
    public class IntegracaoIntercab
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoIntercab(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        //public void IntegrarCTeAnteriorEMPAsync()
        //public async Task IntegrarCTeAnteriorEMPAsync()
        public void IntegrarCTeAnteriorEMPAsync()
        {
            DateTime dataFinal = DateTime.Now.Date.AddDays(-1);
            DateTime dataInicial = DateTime.Now.Date.AddDays(-4);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            int limite = 6000;
            int inicio = 0;

            int totalRegistros = repCTe.ContarConsultaCTesPorPeriodo(dataInicial, dataFinal, true, string.Empty);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.ConsultarCTesPorPeriodo(dataInicial, dataFinal, true, inicio, limite, string.Empty);

            List<Dominio.ObjetosDeValor.WebService.Rest.CTe> listaCteRetorno = ConverterRetornoCTe(listaCTes, _unitOfWork);
            var config = new ProducerConfig
            {
                BootstrapServers = "pkc-lq8gm.westeurope.azure.confluent.cloud:9092",
                SaslUsername = "PIJWNGSIBAQ5FKT4",
                SaslPassword = "V5YunG73nTRRsTO8OEZe+A5pQr/FsXTuB+4TV/QhvcKIxLKWl9lOcKfOh7Tf9MSC",
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                Acks = Acks.Leader,
                SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.Https
            };
            var kafkaMessage = new Message<Null, string>();
            kafkaMessage.Value = JsonConvert.SerializeObject(listaCteRetorno);

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    producer.Produce("ANL.documentation.test.responseBuscarCTePeriodoAnterior.topic.internal.any.v1", kafkaMessage, handler);
                }
                catch (ProduceException<string, string> e)
                {
                    Servicos.Log.TratarErro($"Failed to deliver message: {e.Error.Reason}");
                }
                producer.Flush(TimeSpan.FromSeconds(60));
            }

        }

        public static void handler(DeliveryReport<Null, string> deliveryReport)
        {
            if (deliveryReport.Status == PersistenceStatus.NotPersisted)
            {
                Servicos.Log.TratarErro(deliveryReport.Error?.Reason, "EMP");
            }
        }

        public void IntegrarCargaCompleta(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotasFiscais = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Servicos.WebService.Carga.Carga servicoCarga = new WebService.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.NFe.NFe serNfe = new Servicos.Embarcador.NFe.NFe(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            integracaoPendente.NumeroTentativas += 1;
            integracaoPendente.DataIntegracao = DateTime.Now;

            InspectorBehavior inspectorCTe = new InspectorBehavior();
            string msgRetorno = string.Empty;
            string lastRequestXML = string.Empty;
            string lastResponseXML = string.Empty;

            try
            {
                if (integracaoIntercab == null)
                    throw new ServicoException("Integração da intercab não encontrada!");

                msgRetorno = string.Empty;
                lastRequestXML = string.Empty;
                lastResponseXML = string.Empty;
                inspectorCTe = new InspectorBehavior();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorCarga(integracaoPendente.Carga.Codigo);
                Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoCompleta cargaIntegracaoCompleta = servicoCarga.ConverterCargaDadosTransporte(integracaoPendente.Carga, listaCargaPedido);

                if (!IntegrarCargaCompleta(integracaoIntercab, integracaoPendente, out msgRetorno, out lastRequestXML, out lastResponseXML, cargaIntegracaoCompleta))
                {
                    integracaoPendente.ProblemaIntegracao = msgRetorno;
                    integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");

                    repCargaIntegracao.Atualizar(integracaoPendente);

                    return;
                }
                else
                {
                    integracaoPendente = repCargaIntegracao.BuscarPorCodigo(integracaoPendente.Codigo);
                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");
                }

                msgRetorno = string.Empty;
                lastRequestXML = string.Empty;
                lastResponseXML = string.Empty;
                inspectorCTe = new InspectorBehavior();

                if (!integracaoPendente.Carga.CargaSVM && !IntegrarNotasFiscais(integracaoIntercab, integracaoPendente, out msgRetorno, out lastRequestXML, out lastResponseXML, cargaIntegracaoCompleta))
                {
                    integracaoPendente.ProblemaIntegracao = msgRetorno;
                    integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");

                    repCargaIntegracao.Atualizar(integracaoPendente);

                    return;
                }
                else
                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");

                msgRetorno = string.Empty;
                lastRequestXML = string.Empty;
                lastResponseXML = string.Empty;
                inspectorCTe = new InspectorBehavior();

                if (!IntegrarCTeAnterior(integracaoIntercab, integracaoPendente, out msgRetorno, out lastRequestXML, out lastResponseXML))
                {
                    integracaoPendente.ProblemaIntegracao = msgRetorno;
                    integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");

                    repCargaIntegracao.Atualizar(integracaoPendente);

                    return;
                }
                else
                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");

                msgRetorno = string.Empty;
                lastRequestXML = string.Empty;
                lastResponseXML = string.Empty;
                inspectorCTe = new InspectorBehavior();

                if (!IntegrarCargaDocumentos(integracaoIntercab, integracaoPendente, out msgRetorno, out lastRequestXML, out lastResponseXML))
                {
                    integracaoPendente.ProblemaIntegracao = msgRetorno;
                    integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");

                    repCargaIntegracao.Atualizar(integracaoPendente);

                    return;
                }
                else
                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");


                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                integracaoPendente.ProblemaIntegracao = "Integração de toda a documentação realizada com sucesso";

            }
            catch (ServicoException excecao)
            {
                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Falha ao tentar integrar lista de notas: {ex.Message}", "IntegrarDadosNotasFiscais");

                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = $"Falha ao tentar integrar lista de notas: {ex.Message}";
                servicoArquivoTransacao.Adicionar(integracaoPendente, inspectorCTe.LastRequestXML, inspectorCTe.LastResponseXML, "xml");
            }

            repCargaIntegracao.Atualizar(integracaoPendente);
        }

        public void IntegrarTodosDocumentos(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotasFiscais = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            
            Servicos.WebService.Carga.Carga servicoCarga = new WebService.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.NFe.NFe serNfe = new Servicos.Embarcador.NFe.NFe(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            integracaoPendente.NumeroTentativas += 1;
            integracaoPendente.DataIntegracao = DateTime.Now;

            InspectorBehavior inspectorCTe = new InspectorBehavior();
            string msgRetorno = string.Empty;
            string lastRequestXML = string.Empty;
            string lastResponseXML = string.Empty;

            try
            {
                if (integracaoIntercab == null)
                    throw new ServicoException("Integração da intercab não encontrada!");

                msgRetorno = string.Empty;
                lastRequestXML = string.Empty;
                lastResponseXML = string.Empty;
                inspectorCTe = new InspectorBehavior();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorCarga(integracaoPendente.Carga.Codigo);
                Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoCompleta cargaIntegracaoCompleta = servicoCarga.ConverterCargaDadosTransporte(integracaoPendente.Carga, listaCargaPedido);

                if (!IntegrarNotasFiscais(integracaoIntercab, integracaoPendente, out msgRetorno, out lastRequestXML, out lastResponseXML, cargaIntegracaoCompleta))
                {
                    integracaoPendente.ProblemaIntegracao = msgRetorno;
                    integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");

                    repCargaIntegracao.Atualizar(integracaoPendente);

                    return;
                }
                else
                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");

                msgRetorno = string.Empty;
                lastRequestXML = string.Empty;
                lastResponseXML = string.Empty;
                inspectorCTe = new InspectorBehavior();

                if (!IntegrarCTeAnterior(integracaoIntercab, integracaoPendente, out msgRetorno, out lastRequestXML, out lastResponseXML))
                {
                    integracaoPendente.ProblemaIntegracao = msgRetorno;
                    integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");

                    repCargaIntegracao.Atualizar(integracaoPendente);

                    return;
                }
                else
                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");

                msgRetorno = string.Empty;
                lastRequestXML = string.Empty;
                lastResponseXML = string.Empty;
                inspectorCTe = new InspectorBehavior();

                if (!IntegrarCargaDocumentos(integracaoIntercab, integracaoPendente, out msgRetorno, out lastRequestXML, out lastResponseXML))
                {
                    integracaoPendente.ProblemaIntegracao = msgRetorno;
                    integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");

                    repCargaIntegracao.Atualizar(integracaoPendente);

                    return;
                }
                else
                    servicoArquivoTransacao.Adicionar(integracaoPendente, lastRequestXML, lastResponseXML, "xml");


                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                integracaoPendente.ProblemaIntegracao = "Integração de toda a documentação realizada com sucesso";

            }
            catch (ServicoException excecao)
            {
                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Falha ao tentar integrar lista de notas: {ex.Message}", "IntegrarDadosNotasFiscais");

                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = $"Falha ao tentar integrar lista de notas: {ex.Message}";
                servicoArquivoTransacao.Adicionar(integracaoPendente, inspectorCTe.LastRequestXML, inspectorCTe.LastResponseXML, "xml");
            }

            repCargaIntegracao.Atualizar(integracaoPendente);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracaoPendentes)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Servicos.WebService.Carga.Carga servicoCarga = new WebService.Carga.Carga(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            cargaIntegracaoPendentes.NumeroTentativas += 1;
            cargaIntegracaoPendentes.DataIntegracao = DateTime.Now;
            InspectorBehavior inspectorCTe = new InspectorBehavior();

            try
            {
                if (integracaoIntercab == null)
                    throw new ServicoException("Integração da intercab não encontrada!");

                if (cargaIntegracaoPendentes.Carga.CargaRecebidaDeIntegracao == true)
                {
                    cargaIntegracaoPendentes.ProblemaIntegracao = "Carga recebida de integração.";
                    cargaIntegracaoPendentes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    repositorioCargaDadosTransporteIntegracao.Atualizar(cargaIntegracaoPendentes);

                    return;
                }

                ServicoSGT.Carga.CargasClient svcCarga = ObterClientCarga(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                svcCarga.Endpoint.EndpointBehaviors.Add(inspectorCTe);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorCarga(cargaIntegracaoPendentes.Carga.Codigo);

                if (listaCargaPedido.Count == 0)
                    throw new ServicoException($"Não foram encontrados pedidos para a carga ${cargaIntegracaoPendentes.Carga.Codigo}!");

                ServicoSGT.Carga.RetornoOfint retorno = svcCarga.AdicionarCargaCompleta(servicoCarga.ConverterCargaDadosTransporte(cargaIntegracaoPendentes.Carga, listaCargaPedido));

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = DateTime.Now;
                arquivoIntegracao.Mensagem = "Dados das notas intregados com sucesso.";
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                if (!retorno.Status || retorno.Objeto == 0)
                {
                    arquivoIntegracao.Mensagem = retorno.Mensagem;
                    cargaIntegracaoPendentes.ProblemaIntegracao = retorno.Mensagem;
                    cargaIntegracaoPendentes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    cargaIntegracaoPendentes.ProblemaIntegracao = string.Empty;
                    cargaIntegracaoPendentes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = listaCargaPedido?.FirstOrDefault()?.Carga ?? null;

                    if (carga != null)
                    {
                        carga.CargaProtocoloIntegrada = retorno.Objeto;
                        repositorioCarga.Atualizar(carga);
                    }
                }

                servicoArquivoTransacao.Adicionar(cargaIntegracaoPendentes, inspectorCTe.LastRequestXML, inspectorCTe.LastResponseXML, "xml");
            }
            catch (ServicoException exception)
            {
                cargaIntegracaoPendentes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendentes.ProblemaIntegracao = exception.Message;
            }
            catch (Exception exception)
            {
                Servicos.Log.TratarErro($"Falha ao tentar integrar carga: {exception.Message}", "IntegrarCargaCompleta");

                cargaIntegracaoPendentes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendentes.ProblemaIntegracao = $"Falha ao tentar integrar carga completa: {exception.Message}";
                servicoArquivoTransacao.Adicionar(cargaIntegracaoPendentes, inspectorCTe.LastRequestXML, inspectorCTe.LastResponseXML, "xml");

            }
            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaIntegracaoPendentes);
        }

        public void IntegrarCargaCTeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao)
        {
            if (cargaCTeManualIntegracao.Status == Dominio.Enumeradores.StatusIntegracaoCTeManual.AnularCTe)//Gerencial            
                IntegrarCargaCTeManual_RealizarAnulacaoGerencial(cargaCTeManualIntegracao);
            else //Demais status usam o mesmo método
                IntegrarCargaCTeManual_EnviarCTe(cargaCTeManualIntegracao);
        }

        public void IntegrarCartaCorrecaoCTe(Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao cartaCorrecaoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao repCartaCorrecaoIntegracao = new Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);

            Servicos.WebService.CTe.CCe servicoCCe = new Servicos.WebService.CTe.CCe(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            cartaCorrecaoIntegracao.DataIntegracao = DateTime.Now;
            cartaCorrecaoIntegracao.NumeroTentativas++;

            if (!(integracaoIntercab?.PossuiIntegracaoIntercab ?? false))
            {
                cartaCorrecaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cartaCorrecaoIntegracao.ProblemaIntegracao = "Integração Intercab não configurada!";
                repCartaCorrecaoIntegracao.Atualizar(cartaCorrecaoIntegracao);
                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                ServicoSGT.CTe.CTeClient cteClient = ObterClientCTe(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                cteClient.Endpoint.EndpointBehaviors.Add(inspector);

                Dominio.ObjetosDeValor.WebService.CTe.CCe cce = servicoCCe.ConverterObjetoCCe(cartaCorrecaoIntegracao.CartaCorrecao);
                ServicoSGT.CTe.RetornoOfboolean retorno = cteClient.EnviarCCe(cce);

                if (!retorno.Status)
                    throw new ServicoException(retorno.Mensagem);

                cartaCorrecaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cartaCorrecaoIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException ex)
            {
                cartaCorrecaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cartaCorrecaoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cartaCorrecaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cartaCorrecaoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do Intercab";
            }

            servicoArquivoTransacao.Adicionar(cartaCorrecaoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repCartaCorrecaoIntegracao.Atualizar(cartaCorrecaoIntegracao);
        }

        public void IntegrarOcorrenciaCTe(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;
            ocorrenciaCTeIntegracao.NumeroTentativas++;

            if (!(integracaoIntercab?.PossuiIntegracaoIntercab ?? false))
            {
                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Integração Intercab não configurada!";
                repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                ServicoSGT.Ocorrencia.OcorrenciasClient ocorrenciaClient = ObterClientOcorrencia(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                ocorrenciaClient.Endpoint.EndpointBehaviors.Add(inspector);

                Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti ocorrenciaIntegracao = PreencherOcorrenciaIntegracaoMulti(ocorrenciaCTeIntegracao, _unitOfWork);

                ServicoSGT.Ocorrencia.RetornoOfint retorno = ocorrenciaClient.EnviarOcorrencia(ocorrenciaIntegracao);

                if (ocorrenciaCTeIntegracao.CargaOcorrencia != null)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia;

                    cargaOcorrencia.Protocolo = retorno.Objeto;

                    repositorioCargaOcorrencia.Atualizar(cargaOcorrencia);
                }

                if (retorno.Objeto == 0)
                    throw new ServicoException(retorno.Mensagem);

                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException ex)
            {
                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do Intercab";
            }

            servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
        }

        public void IntegrarCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);

            Servicos.WebService.Carga.Carga servicoCarga = new WebService.Carga.Carga(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            cargaCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCargaIntegracao.NumeroTentativas++;

            if (!(integracaoIntercab?.PossuiIntegracaoIntercab ?? false))
            {
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = "Integração Intercab não configurada!";
                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                ServicoSGT.Carga.CargasClient cargaClient = ObterClientCarga(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                cargaClient.Endpoint.EndpointBehaviors.Add(inspector);

                Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoCarga envioCancelamentoCarga = servicoCarga.ConverterParaObjetoEnvioCancelamentoCarga(cargaCargaIntegracao);
                ServicoSGT.Carga.RetornoOfboolean retorno = cargaClient.EnviarCancelamentoCarga(envioCancelamentoCarga);

                if (!retorno.Status)
                    throw new ServicoException(retorno.Mensagem);

                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCargaIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException ex)
            {
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração de cancelamento do Intercab";
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
            arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
            arquivoIntegracao.Mensagem = cargaCargaIntegracao.ProblemaIntegracao;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
        }

        public void IntegrarCancelamentoOcorrenciaCTe(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao ocorrenciaCancelamentoIntegracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao repositorioOcorrenciaCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            ocorrenciaCancelamentoIntegracao.DataIntegracao = DateTime.Now;
            ocorrenciaCancelamentoIntegracao.NumeroTentativas++;

            if (!(integracaoIntercab?.PossuiIntegracaoIntercab ?? false))
            {
                ocorrenciaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = "Integração Intercab não configurada!";
                repositorioOcorrenciaCancelamentoIntegracao.Atualizar(ocorrenciaCancelamentoIntegracao);
                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                ServicoSGT.Ocorrencia.OcorrenciasClient ocorrenciaClient = ObterClientOcorrencia(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                ocorrenciaClient.Endpoint.EndpointBehaviors.Add(inspector);

                Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento ocorrenciaCancelamento = PreencherOcorrenciaCancelamento(ocorrenciaCancelamentoIntegracao.OcorrenciaCancelamento, ocorrenciaCancelamentoIntegracao, _unitOfWork);

                ServicoSGT.Ocorrencia.RetornoOfboolean retorno = ocorrenciaClient.EnviarCancelamentoOcorrencia(ocorrenciaCancelamento);

                if (!retorno.Status)
                    throw new ServicoException(retorno.Mensagem);

                ocorrenciaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException ex)
            {
                ocorrenciaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do Intercab";
            }

            servicoArquivoTransacao.Adicionar(ocorrenciaCancelamentoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repositorioOcorrenciaCancelamentoIntegracao.Atualizar(ocorrenciaCancelamentoIntegracao);
        }

        public void IntegrarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaIntegracao repositorioFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);

            Servicos.WebService.Financeiro.Financeiro servicoFinanceiro = new Servicos.WebService.Financeiro.Financeiro(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            faturaIntegracao.DataEnvio = DateTime.Now;
            faturaIntegracao.Tentativas++;

            if (!(integracaoIntercab?.PossuiIntegracaoIntercab ?? false))
            {
                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = "Integração Intercab não configurada!";
                repositorioFaturaIntegracao.Atualizar(faturaIntegracao);
                return;
            }

            if (faturaIntegracao.Fatura.Situacao == SituacaoFatura.EmCancelamento || faturaIntegracao.Fatura.Situacao == SituacaoFatura.EmFechamento)
            {
                if (faturaIntegracao.Tentativas <= 100)
                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                else
                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = "Aguardando finalizar o processo atual da fatura.";
                repositorioFaturaIntegracao.Atualizar(faturaIntegracao);
                return;
            }

            if (repositorioFaturaIntegracao.ContemIntegracaPentende(faturaIntegracao.Fatura.Codigo))
            {
                if (faturaIntegracao.Tentativas <= 10)
                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                else
                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = "Aguardando as demais integrações serem concluídas!";
                repositorioFaturaIntegracao.Atualizar(faturaIntegracao);
                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                ServicoSGT.Financeiro.FinanceiroClient financeiroClient = ObterClientFinanceiro(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                financeiroClient.Endpoint.EndpointBehaviors.Add(inspector);

                Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracao objetoFatura = servicoFinanceiro.ConverterObjetoFaturaIntegracao(faturaIntegracao.Fatura);
                ServicoSGT.Financeiro.RetornoOfboolean retorno = financeiroClient.EnviarFaturaCompleta(objetoFatura);

                if (!retorno.Status)
                    throw new ServicoException(retorno.Mensagem);

                faturaIntegracao.Fatura.FaturaIntegracaComSucesso = true;
                repFatura.Atualizar(faturaIntegracao.Fatura);

                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                faturaIntegracao.MensagemRetorno = "Integrado com sucesso.";
            }
            catch (ServicoException ex)
            {
                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = "Ocorreu uma falha ao realizar a integração do Intercab";
            }

            SalvarArquivosIntegracaoFatura(faturaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML);

            repositorioFaturaIntegracao.Atualizar(faturaIntegracao);
        }

        public void IntegrarTitulo(Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao tituloIntegracao)
        {
            Repositorio.Embarcador.Financeiro.TituloIntegracao repositorioTituloIntegracao = new Repositorio.Embarcador.Financeiro.TituloIntegracao(_unitOfWork);

            if (tituloIntegracao.Titulo.BoletoStatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Gerado || string.IsNullOrEmpty(tituloIntegracao.Titulo.CaminhoBoleto))
            {
                tituloIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tituloIntegracao.ProblemaIntegracao = "Integração Intercab não configurada!";
                repositorioTituloIntegracao.Atualizar(tituloIntegracao);
                return;
            }
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            tituloIntegracao.DataIntegracao = DateTime.Now;
            tituloIntegracao.NumeroTentativas++;

            if (!Utilidades.IO.FileStorageService.Storage.Exists(tituloIntegracao.Titulo.CaminhoBoleto))
            {
                tituloIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tituloIntegracao.ProblemaIntegracao = "Boleto não encontrado";
                repositorioTituloIntegracao.Atualizar(tituloIntegracao);
                return;
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();
            InspectorBehavior inspector = new InspectorBehavior();

            string pdfBoletoBase64 = Convert.ToBase64String(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(tituloIntegracao.Titulo.CaminhoBoleto));

            ServicoSGT.Financeiro.FinanceiroClient financeiroClient1 = ObterClientFinanceiro(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
            financeiroClient1.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoSGT.Financeiro.RetornoOfboolean retorno = financeiroClient1.RecebePDF(tituloIntegracao.Titulo.FaturaParcela?.Fatura?.Codigo ?? 0, pdfBoletoBase64, tituloIntegracao.Titulo.NossoNumero);

            if (retorno.Status)
            {
                tituloIntegracao.ProblemaIntegracao = null;
                tituloIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            else
            {
                Log.TratarErro(retorno.Mensagem);

                tituloIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = retorno.Mensagem;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                tituloIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(tituloIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            
            repositorioTituloIntegracao.Atualizar(tituloIntegracao);
        }

        public void IntegrarCargaMDFeAquaviarioManual(Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao cargaMDFeAquaviarioManualIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao repositorioCargaMDFeAquaviarioManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            cargaMDFeAquaviarioManualIntegracao.DataIntegracao = DateTime.Now;
            cargaMDFeAquaviarioManualIntegracao.NumeroTentativas++;

            if (!(integracaoIntercab?.PossuiIntegracaoIntercab ?? false))
            {
                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "Integração Intercab não configurada!";
                repositorioCargaMDFeAquaviarioManualIntegracao.Atualizar(cargaMDFeAquaviarioManualIntegracao);
                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                ServicoSGT.MDFe.MDFeClient mdfeCliente = ObterClientMDFe(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                mdfeCliente.Endpoint.EndpointBehaviors.Add(inspector);

                Dominio.ObjetosDeValor.WebService.MDFe.MDFeAquaviario mdfeAquaviario = PreencherMDFeAquaviario(cargaMDFeAquaviarioManualIntegracao, _unitOfWork);
                ServicoSGT.MDFe.RetornoOfboolean retorno = mdfeCliente.EnviarMDFeAquaviario(mdfeAquaviario);

                if (!retorno.Status)
                    throw new ServicoException(retorno.Mensagem);

                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "Integrado com sucessso.";

                cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.MDFeEnviadoComSucessoPelaIntegracao = true;
                repCargaMDFeManual.Atualizar(cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual);
            }
            catch (ServicoException ex)
            {
                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do Intercab";
            }

            servicoArquivoTransacao.Adicionar(cargaMDFeAquaviarioManualIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repositorioCargaMDFeAquaviarioManualIntegracao.Atualizar(cargaMDFeAquaviarioManualIntegracao);
        }

        public void IntegrarCargaMDFeCancelamentoManual(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao cargaMDFeAquaviarioManualIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao repCargaMDFeManualCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repositorioCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            cargaMDFeAquaviarioManualIntegracao.DataIntegracao = DateTime.Now;
            cargaMDFeAquaviarioManualIntegracao.NumeroTentativas++;

            if (!(integracaoIntercab?.PossuiIntegracaoIntercab ?? false))
            {
                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "Integração Intercab não configurada!";
                repCargaMDFeManualCancelamentoIntegracao.Atualizar(cargaMDFeAquaviarioManualIntegracao);
                return;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManual = cargaMDFeAquaviarioManualIntegracao.CargaMDFeManualCancelamento.CargaMDFeManual != null ? repositorioCargaMDFeManualMDFe.BuscarPrimeiroPorCargaMDFeManual(cargaMDFeAquaviarioManualIntegracao.CargaMDFeManualCancelamento.CargaMDFeManual.Codigo) : null;

            if (cargaMDFeManual == null || cargaMDFeManual.MDFe == null)
            {
                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "Não foi localizado o MDF-e pare realizar a integração!";
                repCargaMDFeManualCancelamentoIntegracao.Atualizar(cargaMDFeAquaviarioManualIntegracao);
                return;
            }
            if (string.IsNullOrWhiteSpace(cargaMDFeManual.MDFe.ProtocoloCancelamento) || !cargaMDFeManual.MDFe.DataCancelamento.HasValue)
            {
                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "O MDF-e ainda não se encontra cancelado!";
                repCargaMDFeManualCancelamentoIntegracao.Atualizar(cargaMDFeAquaviarioManualIntegracao);
                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                ServicoSGT.MDFe.MDFeClient mdfeCliente = ObterClientMDFe(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                mdfeCliente.Endpoint.EndpointBehaviors.Add(inspector);

                ServicoSGT.MDFe.RetornoOfboolean retorno = mdfeCliente.AtualizarSituacaoMDFeAquaviario(cargaMDFeManual.MDFe.Chave, Dominio.Enumeradores.StatusMDFe.Cancelado, cargaMDFeManual.MDFe.ProtocoloCancelamento, cargaMDFeManual.MDFe.DataCancelamento.Value, (cargaMDFeManual.MDFe.MensagemStatus != null ? cargaMDFeManual.MDFe.MensagemStatus.MensagemDoErro : cargaMDFeManual.MDFe.MensagemRetornoSefaz), cargaMDFeAquaviarioManualIntegracao.CargaMDFeManualCancelamento.MotivoCancelamento);

                if (!retorno.Status)
                    throw new ServicoException(retorno.Mensagem);

                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException ex)
            {
                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do Intercab";
            }

            servicoArquivoTransacao.Adicionar(cargaMDFeAquaviarioManualIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repCargaMDFeManualCancelamentoIntegracao.Atualizar(cargaMDFeAquaviarioManualIntegracao);
        }

        public void IntegrarCargaMDFeEncerramentoManual(Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao cargaMDFeAquaviarioManualIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao repCargaMDFeAquaviarioManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repositorioCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            cargaMDFeAquaviarioManualIntegracao.DataIntegracao = DateTime.Now;
            cargaMDFeAquaviarioManualIntegracao.NumeroTentativas++;

            if (!(integracaoIntercab?.PossuiIntegracaoIntercab ?? false))
            {
                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "Integração Intercab não configurada!";
                repCargaMDFeAquaviarioManualIntegracao.Atualizar(cargaMDFeAquaviarioManualIntegracao);
                return;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManual = cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual != null ? repositorioCargaMDFeManualMDFe.BuscarPrimeiroPorCargaMDFeManual(cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.Codigo) : null;

            if (cargaMDFeManual == null || cargaMDFeManual.MDFe == null)
            {
                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "Não foi localizado o MDF-e pare realizar a integração!";
                repCargaMDFeAquaviarioManualIntegracao.Atualizar(cargaMDFeAquaviarioManualIntegracao);
                return;
            }
            if (string.IsNullOrWhiteSpace(cargaMDFeManual.MDFe.ProtocoloEncerramento) || !cargaMDFeManual.MDFe.DataEncerramento.HasValue)
            {
                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "O MDF-e ainda não se encontra encerrado!";
                repCargaMDFeAquaviarioManualIntegracao.Atualizar(cargaMDFeAquaviarioManualIntegracao);
                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                ServicoSGT.MDFe.MDFeClient mdfeCliente = ObterClientMDFe(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                mdfeCliente.Endpoint.EndpointBehaviors.Add(inspector);

                ServicoSGT.MDFe.RetornoOfboolean retorno = mdfeCliente.AtualizarSituacaoMDFeAquaviario(cargaMDFeManual.MDFe.Chave, Dominio.Enumeradores.StatusMDFe.Encerrado, cargaMDFeManual.MDFe.ProtocoloEncerramento, cargaMDFeManual.MDFe.DataEncerramento.Value, (cargaMDFeManual.MDFe.MensagemStatus != null ? cargaMDFeManual.MDFe.MensagemStatus.MensagemDoErro : cargaMDFeManual.MDFe.MensagemRetornoSefaz), "Encerramento do MDF-e");

                if (!retorno.Status)
                    throw new ServicoException(retorno.Mensagem);

                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException ex)
            {
                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaMDFeAquaviarioManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaMDFeAquaviarioManualIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do Intercab";
            }

            servicoArquivoTransacao.Adicionar(cargaMDFeAquaviarioManualIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repCargaMDFeAquaviarioManualIntegracao.Atualizar(cargaMDFeAquaviarioManualIntegracao);
        }

        public void IntegrarArquivoMercanteIntegracao(Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao arquivoMercanteIntegracao)
        {
            Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao repositorioArquivoMercanteIntegracao = new Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            arquivoMercanteIntegracao.DataIntegracao = DateTime.Now;
            arquivoMercanteIntegracao.NumeroTentativas++;

            if (!(integracaoIntercab?.PossuiIntegracaoIntercab ?? false))
            {
                arquivoMercanteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                arquivoMercanteIntegracao.ProblemaIntegracao = "Integração Intercab não configurada!";
                repositorioArquivoMercanteIntegracao.Atualizar(arquivoMercanteIntegracao);
                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                ServicoSGT.CTe.CTeClient cteClient = ObterClientCTe(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                cteClient.Endpoint.EndpointBehaviors.Add(inspector);

                Dominio.ObjetosDeValor.WebService.CTe.DadosDoMercante dadosDoMercante = ConverterDadosDoMercante(arquivoMercanteIntegracao, _unitOfWork);
                ServicoSGT.CTe.RetornoOfboolean retorno = cteClient.SalvarDadosDoMercante(dadosDoMercante);

                if (!retorno.Status)
                    throw new ServicoException(retorno.Mensagem);

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
                arquivoMercanteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do Intercab";
            }

            servicoArquivoTransacao.Adicionar(arquivoMercanteIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repositorioArquivoMercanteIntegracao.Atualizar(arquivoMercanteIntegracao);
        }

        public string ValidarPermissaoCancelarCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool retornarMotivo = false)
        {
            return "";
            //removido por solicitação da Aliança
            //Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            //Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            //Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPeiddoCteSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
            //Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(_unitOfWork);

            //Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            //bool timelinePortoPortoHabilitada = integracaoIntercab?.HabilitarTimelineCargaPortoPorto ?? false;
            //if (!timelinePortoPortoHabilitada)
            //    return "";

            //bool cargaPedidoMercante = carga.Pedidos?.Any(o => o.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto) ?? false;
            //if (!cargaPedidoMercante)
            //    return "";

            //List<int> codigosPedido = (from obj in carga.Pedidos select obj.Codigo).ToList();

            //List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> CTes = repPeiddoCteSubcontratacao.BuscarPorCTePorCarga(carga.Codigo);
            //List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> cargaPedidoXMLNotaFiscalParcial = repCargaPedidoXMLNotaFiscalParcial.BuscarPorCargasPedido(codigosPedido);
            //List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> xmlsNotaFiscal = repPedidoXmlNotaFiscal.BuscarPorCargaPedidos(codigosPedido);

            //bool existeNotas = repPedidoXmlNotaFiscal.VerificarSeExisteNota(carga.Codigo, ClassificacaoNFe.Todos);
            //bool existeCtes = CTes.Count > 0 ? true : false;
            //bool existeCargaPeidodNFParcial = cargaPedidoXMLNotaFiscalParcial.Count > 0 ? true : false;
            //bool existeXmlNotaFiscal = xmlsNotaFiscal.Count > 0 ? true : false;

            //if (timelinePortoPortoHabilitada && cargaPedidoMercante && (existeNotas || existeCtes || existeCargaPeidodNFParcial || existeXmlNotaFiscal))
            //{
            //    if (retornarMotivo)
            //        return "Não é possível atualizar ou cancelar a carga Porto x Porto contendo documentos associado na etapa dois";
            //    else
            //        throw new ServicoException("Não é possível atualizar ou cancelar a carga Porto x Porto contendo documentos associado na etapa dois");
            //}
            //return "";
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool IntegrarNotasFiscais(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente, out string msgRetorno, out string lastRequestXML, out string lastResponseXML, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoCompleta cargaIntegracaoCompleta)
        {
            msgRetorno = string.Empty;
            lastRequestXML = string.Empty;
            lastResponseXML = string.Empty;
            InspectorBehavior inspectorCTe = new InspectorBehavior();

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotasFiscais = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Servicos.Embarcador.NFe.NFe serNfe = new Servicos.Embarcador.NFe.NFe(_unitOfWork);
            Servicos.WebService.Carga.Carga serWSCarga = new WebService.Carga.Carga(_unitOfWork);

            try
            {
                ServicoSGT.NFe.NFeClient svcNFe = ObterClientNFe(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);

                svcNFe.Endpoint.EndpointBehaviors.Add(inspectorCTe);

                foreach (var cargaPedido in integracaoPendente.Carga.Pedidos)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotas = repXmlNotasFiscais.BuscarTodasPorCargaPedido(cargaPedido.Codigo);

                    if (xmlNotas == null || xmlNotas.Count == 0)
                    {
                        msgRetorno = "Sem nota fiscal para realizar a integração";
                        return true;
                    }

                    Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal[] listaObjNotaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal[] { };
                    Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolos = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos()
                    {
                        protocoloIntegracaoPedido = 0,
                        protocoloIntegracaoCarga = integracaoPendente.Carga?.CargaProtocoloIntegrada ?? 0,
                        NumeroContainerPedido = cargaPedido.Pedido.Container?.Numero ?? "",
                        TaraContainer = cargaPedido.Pedido?.TaraContainer ?? "",
                        LacreContainerUm = cargaPedido.Pedido?.LacreContainerUm ?? "",
                        LacreContainerDois = cargaPedido.Pedido?.LacreContainerDois ?? "",
                        LacreContainerTres = cargaPedido.Pedido?.LacreContainerTres ?? "",
                        Container = cargaPedido.Pedido?.Container != null ? serWSCarga.ConverterObjetoContainer(cargaPedido.Pedido.Container) : null
                    };

                    List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> listaNotas = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota in xmlNotas)
                    {
                        Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nf = serNfe.ConverterXMLEmNota(nota, string.Empty, _unitOfWork, carga: integracaoPendente.Carga);
                        listaNotas.Add(nf);
                    }
                    listaObjNotaFiscal = listaNotas.ToArray<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();

                    if (listaObjNotaFiscal.Length == 0)
                    {
                        msgRetorno = "Não possui notas para integrar";
                        return true;
                    }

                    ServicoSGT.NFe.RetornoOfboolean retorno = svcNFe.IntegrarDadosNotasFiscais(protocolos, listaObjNotaFiscal, cargaIntegracaoCompleta);

                    lastRequestXML = inspectorCTe.LastRequestXML;
                    lastResponseXML = inspectorCTe.LastResponseXML;

                    if (!retorno.Status)
                    {
                        msgRetorno = retorno.Mensagem;
                        return false;
                    }
                }
                msgRetorno = "Dados das notas intregados com sucesso.";
                return true;
            }
            catch (ServicoException excecao)
            {
                msgRetorno = excecao.Message;
                return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Falha ao tentar integrar lista de notas: {ex.Message}", "IntegrarDadosNotasFiscais");

                msgRetorno = $"Falha ao tentar integrar lista de notas: {ex.Message}";
                return false;
            }
        }

        private bool IntegrarCTeAnterior(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente, out string msgRetorno, out string lastRequestXML, out string lastResponseXML)
        {
            msgRetorno = string.Empty;
            lastRequestXML = string.Empty;
            lastResponseXML = string.Empty;
            InspectorBehavior inspectorCTe = new InspectorBehavior();

            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(_unitOfWork);
            Servicos.Embarcador.Carga.CTePorCTeParaSubcontratacao serCTePorCTeParaSubcontratacao = new Servicos.Embarcador.Carga.CTePorCTeParaSubcontratacao(_unitOfWork);
            Servicos.WebService.Carga.Carga serWSCarga = new WebService.Carga.Carga(_unitOfWork);

            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

            try
            {
                ServicoSGT.CTe.CTeClient svcCTe = ObterClientCTe(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);

                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctes = repPedidoCTeParaSubContratacao.BuscarPorCTePorCarga(integracaoPendente.Carga.Codigo);

                if (ctes == null || ctes.Count == 0)
                {
                    msgRetorno = "Sem CT-e anterior para realizar a integração";
                    return true;
                }

                svcCTe.Endpoint.EndpointBehaviors.Add(inspectorCTe);

                foreach (var cargaPedido in integracaoPendente.Carga.Pedidos)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe[] listaObjCTes = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe[] { };
                    List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> listaCTes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();

                    foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cte in ctes.Where(cte => cargaPedido.CargaPedidoDocumentosCTe?.Any(cpe => cpe.CTe.Codigo == cte.Codigo) ?? false))
                        listaCTes.Add(serCTePorCTeParaSubcontratacao.ConverterCTeTerceiroParaObjeto(cte, enviarCTeApenasParaTomador, _unitOfWork));

                    listaObjCTes = listaCTes.ToArray<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();

                    if (listaObjCTes.Length == 0)
                    {
                        msgRetorno = "Não possui CT-es anteriores para integrar.";
                        return true;
                    }

                    Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolos = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos()
                    {
                        protocoloIntegracaoPedido = 0,
                        protocoloIntegracaoCarga = integracaoPendente.Carga?.CargaProtocoloIntegrada ?? 0,
                        NumeroContainerPedido = cargaPedido.Pedido?.Container?.Numero ?? "",
                        TaraContainer = cargaPedido.Pedido?.TaraContainer ?? "",
                        LacreContainerUm = cargaPedido.Pedido?.LacreContainerUm ?? "",
                        LacreContainerDois = cargaPedido.Pedido?.LacreContainerDois ?? "",
                        LacreContainerTres = cargaPedido.Pedido?.LacreContainerTres ?? "",
                        Container = cargaPedido.Pedido?.Container != null ? serWSCarga.ConverterObjetoContainer(cargaPedido.Pedido.Container) : null
                    };

                    ServicoSGT.CTe.RetornoOfboolean retorno = svcCTe.IntegrarDadosCTesAnteriores(protocolos, listaObjCTes);

                    lastRequestXML = inspectorCTe.LastRequestXML;
                    lastResponseXML = inspectorCTe.LastResponseXML;

                    if (!retorno.Status)
                    {
                        msgRetorno = retorno.Mensagem;
                        return false;
                    }
                }

                msgRetorno = "CT-es anteriores integrados com sucesso.";
                return true;
            }
            catch (ServicoException excecao)
            {
                msgRetorno = excecao.Message;
                return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Falha ao tentar integrar lista de notas: {ex.Message}", "IntegrarDadosNotasFiscais");

                msgRetorno = $"Falha ao tentar integrar lista de notas: {ex.Message}";
                return false;
            }
        }

        private bool IntegrarCargaDocumentos(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente, out string msgRetorno, out string lastRequestXML, out string lastResponseXML)
        {
            msgRetorno = string.Empty;
            lastRequestXML = string.Empty;
            lastResponseXML = string.Empty;
            InspectorBehavior inspectorCTe = new InspectorBehavior();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(_unitOfWork);
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
            Servicos.Embarcador.Carga.CTePorCTeParaSubcontratacao serCTePorCTeParaSubcontratacao = new Servicos.Embarcador.Carga.CTePorCTeParaSubcontratacao(_unitOfWork);

            try
            {
                ServicoSGT.Carga.CargasClient svcCarga = ObterClientCarga(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();

                svcCarga.Endpoint.EndpointBehaviors.Add(inspectorCTe);

                Dominio.ObjetosDeValor.WebService.Carga.CargaDocumentos cargaDocumentos = new Dominio.ObjetosDeValor.WebService.Carga.CargaDocumentos();
                cargaDocumentos.ProtocoloCarga = integracaoPendente.Carga?.CargaProtocoloIntegrada ?? 0;
                cargaDocumentos.PesoBruto = repCargaPedido.BuscarPesoTotalPorCarga(integracaoPendente.Carga.Codigo);
                cargaDocumentos.PesoLiquido = repCargaPedido.BuscarPesoLiquidoTotalPorCarga(integracaoPendente.Carga.Codigo);
                cargaDocumentos.Conhecimentos = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ctes = repCargaCTe.BuscarPorCarga(integracaoPendente.Carga.Codigo, true);
                List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTes((from obj in ctes select obj.CTe.Codigo).ToList());

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cte in ctes)
                    cargaDocumentos.Conhecimentos.Add(serCTe.ConverterObjetoCargaCTe(cte, cTeContaContabilContabilizacaos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, _unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, configuracaoTMS, configuracaoCargaIntegracao));

                ServicoSGT.Carga.RetornoOfboolean retorno = svcCarga.EnviarCargaDocumentos(cargaDocumentos);

                lastRequestXML = inspectorCTe.LastRequestXML;
                lastResponseXML = inspectorCTe.LastResponseXML;

                if (!retorno.Status)
                {
                    msgRetorno = retorno.Mensagem;
                    return false;
                }
                else
                {
                    msgRetorno = "Documentação da carga intregado com sucesso.";
                    return true;
                }
            }
            catch (ServicoException excecao)
            {
                msgRetorno = excecao.Message;
                return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Falha ao tentar integrar lista de notas: {ex.Message}", "IntegrarDadosNotasFiscais");

                msgRetorno = $"Falha ao tentar integrar lista de notas: {ex.Message}";
                return false;
            }
        }

        private void IntegrarCargaCTeManual_EnviarCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);


            Servicos.Embarcador.CTe.CTe servicoCTe = new CTe.CTe(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

            cargaCTeManualIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeManualIntegracao.NumeroTentativas++;
            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

            if (!(integracaoIntercab?.PossuiIntegracaoIntercab ?? false))
            {
                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeManualIntegracao.ProblemaIntegracao = "Integração Intercab não configurada!";
                repCargaCTeManualIntegracao.Atualizar(cargaCTeManualIntegracao);
                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                ServicoSGT.CTe.CTeClient cteClient = ObterClientCTe(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                cteClient.Endpoint.EndpointBehaviors.Add(inspector);

                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = servicoCTe.ConverterEntidadeCTeParaObjeto(cargaCTeManualIntegracao.CTe, enviarCTeApenasParaTomador, _unitOfWork);
                ServicoSGT.CTe.RetornoOfboolean retorno = cteClient.EnviarCTe(cte, cargaCTeManualIntegracao.Carga.CargaProtocoloIntegrada);

                if (!retorno.Status)
                    throw new ServicoException(retorno.Mensagem);

                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCTeManualIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException ex)
            {
                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeManualIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeManualIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do Intercab";
            }

            servicoArquivoTransacao.Adicionar(cargaCTeManualIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repCargaCTeManualIntegracao.Atualizar(cargaCTeManualIntegracao);
        }

        private void IntegrarCargaCTeManual_RealizarAnulacaoGerencial(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

            Servicos.Embarcador.CTe.CTe servicoCTe = new CTe.CTe(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            cargaCTeManualIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeManualIntegracao.NumeroTentativas++;

            if (!(integracaoIntercab?.PossuiIntegracaoIntercab ?? false))
            {
                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeManualIntegracao.ProblemaIntegracao = "Integração Intercab não configurada!";
                repCargaCTeManualIntegracao.Atualizar(cargaCTeManualIntegracao);
                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                ServicoSGT.CTe.CTeClient cteClient = ObterClientCTe(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                cteClient.Endpoint.EndpointBehaviors.Add(inspector);

                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = servicoCTe.ConverterEntidadeCTeParaObjeto(cargaCTeManualIntegracao.CTe, enviarCTeApenasParaTomador, _unitOfWork);
                Dominio.ObjetosDeValor.WebService.CTe.RequestAnulacaoGerencial requestAnulacaoGerencial = new Dominio.ObjetosDeValor.WebService.CTe.RequestAnulacaoGerencial()
                {
                    ChaveCte = cte.Chave,
                    Motivo = "CTE ANULADO GERENCIALMENTE NA BASE ORIGINAL",
                    ProtocoloCarga = cargaCTeManualIntegracao.Carga.CargaProtocoloIntegrada
                };

                ServicoSGT.CTe.RetornoOfboolean retorno = cteClient.RealizarAnulacaoGerencial(requestAnulacaoGerencial);

                if (!retorno.Status)
                    throw new ServicoException(retorno.Mensagem);

                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCTeManualIntegracao.ProblemaIntegracao = "Integrado com sucessso.";
            }
            catch (ServicoException ex)
            {
                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeManualIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeManualIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do Intercab";
            }

            servicoArquivoTransacao.Adicionar(cargaCTeManualIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repCargaCTeManualIntegracao.Atualizar(cargaCTeManualIntegracao);
        }

        private bool IntegrarCargaCompleta(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente, out string msgRetorno, out string lastRequestXML, out string lastResponseXML, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoCompleta cargaIntegracaoCompleta)
        {
            msgRetorno = string.Empty;
            lastRequestXML = string.Empty;
            lastResponseXML = string.Empty;
            InspectorBehavior inspectorCTe = new InspectorBehavior();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Servicos.WebService.Carga.Carga servicoCarga = new WebService.Carga.Carga(_unitOfWork);

            try
            {
                ServicoSGT.Carga.CargasClient svcCarga = ObterClientCarga(integracaoIntercab.URLIntercab, integracaoIntercab.TokenIntercab);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();

                svcCarga.Endpoint.EndpointBehaviors.Add(inspectorCTe);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorCarga(integracaoPendente.Carga.Codigo);

                if (listaCargaPedido.Count == 0)
                    throw new ServicoException($"Não foram encontrados pedidos para a carga ${integracaoPendente.Carga.Codigo}!");

                ServicoSGT.Carga.RetornoOfint retorno = svcCarga.AdicionarCargaCompleta(cargaIntegracaoCompleta);

                lastRequestXML = inspectorCTe.LastRequestXML;
                lastResponseXML = inspectorCTe.LastResponseXML;

                if (!retorno.Status || retorno.Objeto == 0)
                {
                    msgRetorno = retorno.Mensagem;
                    return false;
                }
                else
                {
                    if (integracaoPendente.Carga != null)
                    {
                        integracaoPendente.Carga.CargaProtocoloIntegrada = retorno.Objeto;
                        repCarga.Atualizar(integracaoPendente.Carga);
                    }
                    msgRetorno = "Carga completa criada com sucesso.";
                    return true;
                }
            }
            catch (ServicoException excecao)
            {
                msgRetorno = excecao.Message;
                return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Falha ao tentar integrar lista de notas: {ex.Message}", "IntegrarCargaCompleta");

                msgRetorno = $"Falha ao tentar integrar a carga completa: {ex.Message}";
                return false;
            }
        }

        private void SalvarArquivosIntegracaoFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string arquivoRequisicao, string arquivoRetorno)
        {
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = AdicionarArquivoTransacaoFatura(faturaIntegracao, arquivoRequisicao, arquivoRetorno, faturaIntegracao.MensagemRetorno);

            if (faturaIntegracaoArquivo == null)
                return;

            if (faturaIntegracao.ArquivosIntegracao == null)
                faturaIntegracao.ArquivosIntegracao = new List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo>();

            faturaIntegracao.ArquivosIntegracao.Add(faturaIntegracaoArquivo);
        }

        private Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo AdicionarArquivoTransacaoFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string arquivoRequisicao, string arquivoRetorno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(arquivoRequisicao) && string.IsNullOrWhiteSpace(arquivoRetorno))
                return null;

            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repositorioFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(arquivoRequisicao, "xml", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(arquivoRetorno, "xml", _unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioFaturaIntegracaoArquivo.Inserir(faturaIntegracaoArquivo);

            return faturaIntegracaoArquivo;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Conversão Objetos

        private List<Dominio.ObjetosDeValor.WebService.Rest.CTe> ConverterRetornoCTe(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.Rest.CTe> CTesIntegracao = new List<Dominio.ObjetosDeValor.WebService.Rest.CTe>();

            if (listaCTes.Count > 0)
            {
                foreach (var cte in listaCTes)
                {
                    Dominio.ObjetosDeValor.WebService.Rest.CTe cteIntegracao = new Dominio.ObjetosDeValor.WebService.Rest.CTe();
                    cteIntegracao.ProtocoloCTe = cte.Codigo;
                    cteIntegracao.ChaveCTe = cte.Chave;
                    cteIntegracao.CNPJEmissor = cte.Empresa.CNPJ;
                    cteIntegracao.DataEmissao = cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm");
                    cteIntegracao.DataEvento = cte.DataRetornoSefaz.HasValue ? cte.DataRetornoSefaz.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
                    cteIntegracao.MensagemSefaz = cte.Status != "A" ? cte.MensagemStatus?.MensagemDoErro ?? cte.MensagemRetornoSefaz : string.Empty;
                    cteIntegracao.SituacaoCTe = cte.DescricaoStatus;
                    cteIntegracao.ValorCTe = cte.ValorAReceber;
                    cteIntegracao.NumeroCarga = cte.CargaCTes?.FirstOrDefault().Carga.CodigoCargaEmbarcador ?? string.Empty;

                    CTesIntegracao.Add(cteIntegracao);
                }
            }

            return CTesIntegracao;

        }

        private Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento PreencherOcorrenciaCancelamento(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao ocorrenciaCancelamentoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.CTe.CTe servicoWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repositorioCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repositorioConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repositorioConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            int codigoOcorrenciaCTeIntegracao = ocorrenciaCancelamentoIntegracao?.OcorrenciaCancelamento?.Ocorrencia?.Codigo ?? 0;

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ocorrenciaCTeIntegracoes = codigoOcorrenciaCTeIntegracao > 0 ? repositorioOcorrenciaCTeIntegracao.BuscarPorOcorrencia(codigoOcorrenciaCTeIntegracao) : null;

            List<Dominio.ObjetosDeValor.WebService.CTe.CTe> ctes = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();

            if (ocorrenciaCTeIntegracoes?.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao in ocorrenciaCTeIntegracoes)
                {
                    if (ocorrenciaCTeIntegracao.CargaCTe != null)
                    {
                        List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repositorioCTeContaContabilContabilizacao.BuscarPorCTe(ocorrenciaCTeIntegracao.CargaCTe.CTe.Codigo);
                        ctes.Add(servicoWSCTe.ConverterObjetoCargaCTe(ocorrenciaCTeIntegracao.CargaCTe, cTeContaContabilContabilizacaos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, _unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, configuracaoTMS, configuracaoCargaIntegracao));
                    }
                }
            }

            Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento ocorrenciaCancelamentoObjeto = new Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento()
            {
                ProtocoloCancelamento = ocorrenciaCancelamento?.Codigo ?? 0,
                ProtocoloOcorrecia = ocorrenciaCancelamento?.Ocorrencia?.Protocolo ?? 0,
                DataCancelamento = ocorrenciaCancelamento?.DataCancelamento ?? DateTime.MinValue,
                MotivoCancelamento = ocorrenciaCancelamento?.MotivoCancelamento ?? string.Empty,
                PossuiDocumentoCancelado = ctes?.Count > 0 ? true : false,
                Conhecimentos = ctes
            };

            return ocorrenciaCancelamentoObjeto;
        }

        private Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti PreencherOcorrenciaIntegracaoMulti(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.CTe.CTe servicoWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork);
            Servicos.WebService.Empresa.Empresa servicoWSEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repositorioCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repositorioConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repositorioConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia tipoOcorrencia = PreencherTipoOcorrencia(ocorrenciaCTeIntegracao, unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresa = ocorrenciaCTeIntegracao.CargaOcorrencia.Emitente != null ? servicoWSEmpresa.ConverterObjetoEmpresa(ocorrenciaCTeIntegracao.CargaOcorrencia.Emitente) : null;
            List<Dominio.ObjetosDeValor.WebService.CTe.CTe> ctes = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();

            if (ocorrenciaCTeIntegracao.CargaOcorrencia != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repositorioCargaCTeComplementoInfo.BuscarCTesPorOcorrencia((ocorrenciaCTeIntegracao?.CargaOcorrencia?.Codigo ?? 0), false);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> conhecimentosOcorrenciasComplementares = cargaCTesComplementoInfo.Select(o => o.CTe).ToList();
                List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repositorioCTeContaContabilContabilizacao.BuscarPorCTes((from obj in conhecimentosOcorrenciasComplementares select obj.Codigo).ToList());

                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in conhecimentosOcorrenciasComplementares)
                    ctes.Add(servicoWSCTe.ConverterObjetoCTe(cte, cTeContaContabilContabilizacaos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, _unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF));
            }

            Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti ocorrenciaIntegracao = new Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti()
            {
                Protocolo = ocorrenciaCTeIntegracao.CargaOcorrencia.Protocolo,
                NumeroOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.NumeroOcorrencia,
                Descricao = ocorrenciaCTeIntegracao.CargaOcorrencia.Descricao,
                TipoOcorrencia = tipoOcorrencia,
                Empresa = empresa,
                ProtocoloCarga = ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.CargaProtocoloIntegrada,
                NumeroCargaEmbarcador = ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.CodigoCargaEmbarcador,
                ValorOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.ValorOcorrencia,
                Observacao = ocorrenciaCTeIntegracao.CargaOcorrencia.Observacao,
                DataOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.DataOcorrencia,
                Conhecimentos = ctes
            };

            return ocorrenciaIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia PreencherTipoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia tipoOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia?.TipoOcorrencia != null ? new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia()
            {
                Descricao = ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia?.Descricao ?? string.Empty,
                CodigoIntegracao = ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia?.CodigoIntegracao ?? string.Empty
            } : null;

            return tipoOcorrencia;
        }

        private Dominio.ObjetosDeValor.WebService.MDFe.MDFeAquaviario PreencherMDFeAquaviario(Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao cargaMDFeAquaviarioManualIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.MDFe.MDFe servicoWSMDFe = new Servicos.WebService.MDFe.MDFe(unitOfWork);

            Dominio.ObjetosDeValor.WebService.MDFe.MDFe mdfe = PreencherMDFeMDFeAquaviario(cargaMDFeAquaviarioManualIntegracao, unitOfWork);
            Dominio.ObjetosDeValor.Localidade localidadeOrigem = PreencherLocalidadeMDFeAquaviario(cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.Origem, unitOfWork);
            Dominio.ObjetosDeValor.Localidade localidadeDestino = PreencherLocalidadeMDFeAquaviario(cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.Destino, unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Carga.Porto portoOrigem = PreencherPortoMDFeAquaviario(cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.PortoOrigem, unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Carga.Porto portoDestino = PreencherPortoMDFeAquaviario(cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.PortoDestino, unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Carga.Viagem viagem = PreencherViagemMDFeAquaviario(cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.PedidoViagemNavio, unitOfWork);
            Dominio.ObjetosDeValor.Empresa empresa = PreencherEmpresaEmitenteMDFeAquaviario(cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.Empresa, unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto> tipoTerminalOrigem = PreencherTipoTerminalImportacaoMDFeAquaviario(cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.TerminalCarregamento.Select(o => o.Codigo).ToList(), unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto> tipoTerminalDestino = PreencherTipoTerminalImportacaoMDFeAquaviario(cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.TerminalDescarregamento.Select(o => o.Codigo).ToList(), unitOfWork);

            Dominio.ObjetosDeValor.WebService.MDFe.MDFeAquaviario mdfeAquaviario = new Dominio.ObjetosDeValor.WebService.MDFe.MDFeAquaviario()
            {
                MDFe = mdfe,
                TerminaisDestino = tipoTerminalDestino,
                TerminaisOrigem = tipoTerminalOrigem,
                TransportadoraEmitente = empresa,
                Viagem = viagem,
                PortoEmbarque = portoOrigem,
                PortoDesembarque = portoDestino,
                Origem = localidadeOrigem,
                Destino = localidadeDestino,
                TipoModalMDFe = cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.TipoModalMDFe,
                UsarDadosCTe = cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.UsarDadosCTe,
                UsarSeguroCTe = cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.UsarSeguroCTe,
            };

            return mdfeAquaviario;
        }

        private Dominio.ObjetosDeValor.WebService.MDFe.MDFe PreencherMDFeMDFeAquaviario(Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao cargaMDFeAquaviarioManualIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.MDFe.MDFe servicoWSMDFe = new Servicos.WebService.MDFe.MDFe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repositorioCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unitOfWork);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repositorioDocumentoMunicipoDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManual = cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual != null ? repositorioCargaMDFeManualMDFe.BuscarPrimeiroPorCargaMDFeManual(cargaMDFeAquaviarioManualIntegracao.CargaMDFeManual.Codigo) : null;

            Dominio.ObjetosDeValor.WebService.MDFe.MDFe mdfe = servicoWSMDFe.ConverterObjetoCargaMDFeManual(cargaMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos, unitOfWork, false);

            mdfe.ChavesDeCTe = repositorioDocumentoMunicipoDescarregamentoMDFe.BuscarChavesDeCTesPorMDFe(cargaMDFeManual.MDFe.Codigo);
            mdfe.ProtocoloAutorizacao = cargaMDFeManual.MDFe.Protocolo;
            mdfe.DataAutorizacao = cargaMDFeManual.MDFe.DataAutorizacao.HasValue ? cargaMDFeManual.MDFe.DataAutorizacao.Value : DateTime.MinValue;
            mdfe.MensagemRetornoSefaz = (cargaMDFeManual.MDFe.MensagemStatus != null ? cargaMDFeManual.MDFe.MensagemStatus.MensagemDoErro : cargaMDFeManual.MDFe.MensagemRetornoSefaz);

            return mdfe;
        }

        private Dominio.ObjetosDeValor.Localidade PreencherLocalidadeMDFeAquaviario(Dominio.Entidades.Localidade localidade, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Servicos.Embarcador.Localidades.Localidade(unitOfWork);

            Dominio.ObjetosDeValor.Localidade objetoLocalidade = servicoLocalidade.ConverterObjetoLocalidade(localidade);

            return objetoLocalidade;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.Porto PreencherPortoMDFeAquaviario(Dominio.Entidades.Embarcador.Pedidos.Porto porto, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.Carga.Carga servicoWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Carga.Porto objetoPorto = servicoWSCarga.ConverterObjetoPorto(porto);

            return objetoPorto;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.Viagem PreencherViagemMDFeAquaviario(Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.Carga.Carga servicoWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Carga.Viagem objetoViagem = servicoWSCarga.ConverterObjetoViagem(viagem);

            return objetoViagem;
        }

        private Dominio.ObjetosDeValor.Empresa PreencherEmpresaEmitenteMDFeAquaviario(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Empresa servicoEmpresa = new Servicos.Empresa(unitOfWork);

            Dominio.ObjetosDeValor.Empresa objetoEmpresa = servicoEmpresa.ConverterObjetoEmpresa(empresa);

            return objetoEmpresa;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto> PreencherTipoTerminalImportacaoMDFeAquaviario(List<int> codigosTerminal, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.Carga.Carga servicoWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);

            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repositorioTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto> listaRetornoTipoTerminalImportacao = new List<Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto>();

            List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> listaTipoTerminalImportacao = codigosTerminal.Count > 0 ? repositorioTipoTerminalImportacao.BuscarPorCodigos(codigosTerminal) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao tipoTerminalImportacao in listaTipoTerminalImportacao)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto tipoTerminalImportacaoObjetoConvertido = servicoWSCarga.ConverterObjetoTerminalPorto(tipoTerminalImportacao);

                listaRetornoTipoTerminalImportacao.Add(tipoTerminalImportacaoObjetoConvertido);
            }

            return listaRetornoTipoTerminalImportacao;
        }

        private Dominio.ObjetosDeValor.WebService.CTe.DadosDoMercante ConverterDadosDoMercante(Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao arquivoMercanteIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.WebService.CTe.DadosDoMercante dadosDoMercante = new Dominio.ObjetosDeValor.WebService.CTe.DadosDoMercante()
            {
                ChaveCte = arquivoMercanteIntegracao.ConhecimentoDeTransporteEletronico.ChaveAcesso,
                NumeroCe = arquivoMercanteIntegracao.ConhecimentoDeTransporteEletronico.NumeroCEMercante,
                NumeroManifesto = arquivoMercanteIntegracao.ConhecimentoDeTransporteEletronico.NumeroManifesto,
                NumeroManifestoTransbordo = arquivoMercanteIntegracao.ConhecimentoDeTransporteEletronico.NumeroManifestoTransbordo,
            };

            return dadosDoMercante;
        }

        #endregion Métodos Privados - Conversão Objetos

        #region Métodos Privados - Client

        private ServicoSGT.Carga.CargasClient ObterClientCarga(string url, string token)
        {
#if DEBUG
            url = "http://localhost:5146/";
#endif
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "Cargas.svc";

            ServicoSGT.Carga.CargasClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.Carga.CargasClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.Carga.CargasClient(binding, endpointAddress);
            }
            AdicionarHeaders(token, client.InnerChannel);

            return client;
        }

        private ServicoSGT.NFe.NFeClient ObterClientNFe(string url, string token)
        {
#if DEBUG
            url = "http://localhost:5146/";
#endif
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "NFe.svc";

            ServicoSGT.NFe.NFeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.NFe.NFeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.NFe.NFeClient(binding, endpointAddress);
            }
            AdicionarHeaders(token, client.InnerChannel);
            return client;
        }

        private ServicoSGT.CTe.CTeClient ObterClientCTe(string url, string token)
        {
#if DEBUG
            url = "http://localhost:5146/";
#endif
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "CTe.svc";

            ServicoSGT.CTe.CTeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.CTe.CTeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.CTe.CTeClient(binding, endpointAddress);
            }

            AdicionarHeaders(token, client.InnerChannel);

            return client;
        }

        private ServicoSGT.Ocorrencia.OcorrenciasClient ObterClientOcorrencia(string url, string token)
        {
#if DEBUG
            url = "http://localhost:5146/";
#endif
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "Ocorrencias.svc";

            ServicoSGT.Ocorrencia.OcorrenciasClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.Ocorrencia.OcorrenciasClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.Ocorrencia.OcorrenciasClient(binding, endpointAddress);
            }
            AdicionarHeaders(token, client.InnerChannel);

            return client;
        }

        private ServicoSGT.Financeiro.FinanceiroClient ObterClientFinanceiro(string url, string token)
        {
#if DEBUG
            url = "http://localhost:5146/";
#endif
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "Financeiro.svc";

            ServicoSGT.Financeiro.FinanceiroClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.Financeiro.FinanceiroClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.Financeiro.FinanceiroClient(binding, endpointAddress);
            }
            AdicionarHeaders(token, client.InnerChannel);

            return client;
        }

        private System.ServiceModel.BasicHttpBinding ObterBinding(string url)
        {
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return binding;
        }

        private void AdicionarHeaders(string token, System.ServiceModel.IContextChannel channel)
        {
            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(channel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);
        }

        private ServicoSGT.MDFe.MDFeClient ObterClientMDFe(string url, string token)
        {
#if DEBUG
            url = "http://localhost:5146/";
#endif
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "MDFe.svc";

            ServicoSGT.MDFe.MDFeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.MDFe.MDFeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.MDFe.MDFeClient(binding, endpointAddress);
            }
            AdicionarHeaders(token, client.InnerChannel);

            return client;
        }

        #endregion Métodos Privados - Client
    }
}
