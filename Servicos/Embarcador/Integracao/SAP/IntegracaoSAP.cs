using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.SAP;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml;
using WSSAPNSFE = Servicos.ServiceSAP;

namespace Servicos.Embarcador.Integracao.SAP
{
    public class IntegracaoSAP
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSapAPI4 _configuracaoIntegracaoSAP_API4;

        #endregion

        #region Construtores

        public IntegracaoSAP(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoPendente)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            cargaIntegracaoPendente.NumeroTentativas++;
            cargaIntegracaoPendente.DataIntegracao = DateTime.Now;

            string mensagemRetorno = string.Empty;

            InspectorBehavior inspectorArmazena = new InspectorBehavior();
            bool sucesso = true;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoSAP();
                Servicos.ServicoSAP.ArmazenaCTe.SI_Armazena_Cte_Async_OUTClient clientArmazenaCTe = ObterClientArmazenaCTe(configuracaoIntegracao.URL, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);

                clientArmazenaCTe.Endpoint.EndpointBehaviors.Add(inspectorArmazena);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTEs = cargaIntegracaoPendente.Carga.CargaCTes.Select(o => o.CTe).ToList();

                if (string.IsNullOrEmpty(cargaIntegracaoPendente.Protocolo) || cargaIntegracaoPendente.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                {
                    #region Passo 1
                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in CTEs)
                    {
                        if (cte.ModeloDocumentoFiscal.Codigo == 5)
                        {
                            Servicos.ServicoSAP.ArmazenaCTe.DT_Armazena_Cte_req request = PreencherRequest(cte.Codigo);
                            Servicos.ServicoSAP.ArmazenaCTe.SI_Armazena_Cte_Async_OUTResponse response = clientArmazenaCTe.SI_Armazena_Cte_Async_OUTAsync(request).Result;

                            if (!string.IsNullOrWhiteSpace(response.MT_Armazena_CTe_resp.T_RETORN.MENSAGEM))
                                mensagemRetorno = response.MT_Armazena_CTe_resp.T_RETORN.MENSAGEM;

                            servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, inspectorArmazena.LastRequestXML, inspectorArmazena.LastResponseXML, "xml", $"Armazena CTe ({cte.Numero}) {mensagemRetorno}");
                        }
                        else if (cte.ModeloDocumentoFiscal.Codigo == 30)
                        {
                            //Armazenar nfse
                            Servicos.ServicoSAP.ArmazenaCTe.DT_Armazena_Cte_req request = PreencherRequestNFSe(cte.Codigo);
                            Servicos.ServicoSAP.ArmazenaCTe.SI_Armazena_Cte_Async_OUTResponse response = clientArmazenaCTe.SI_Armazena_Cte_Async_OUTAsync(request).Result;

                            if (!string.IsNullOrWhiteSpace(response.MT_Armazena_CTe_resp.T_RETORN.MENSAGEM))
                                mensagemRetorno = response.MT_Armazena_CTe_resp.T_RETORN.MENSAGEM;

                            servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, inspectorArmazena.LastRequestXML, inspectorArmazena.LastResponseXML, "xml", $"Armazena NFSe ({cte.Numero}) {mensagemRetorno}");

                        }

                    }

                    Servicos.ServicoSAP.EnviaVendaFrete.SI_Envia_Venda_Frete_Sync_OUTClient clientVendaFrete = ObterClientVendaFrete(configuracaoIntegracao.URLEnviaVendaFrete, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                    InspectorBehavior inspectorVendaFrete = new InspectorBehavior();
                    clientVendaFrete.Endpoint.EndpointBehaviors.Add(inspectorVendaFrete);

                    WSSAPNSFE.VendaFrete.SI_Venda_Servico_Sync_OUTClient clientVendaFreteNFSe = ObterClientVendaFreteNFSE(configuracaoIntegracao.URLEnviaVendaFrete, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                    InspectorBehavior inspectorVendaFreteNFSE = new InspectorBehavior();
                    clientVendaFreteNFSe.Endpoint.EndpointBehaviors.Add(inspectorVendaFrete);

                    ServicoSAP.EnviaVendaFrete.ZBRSDS0027[] enviaDadosCRT = new ServicoSAP.EnviaVendaFrete.ZBRSDS0027[] { };
                    ServicoSAP.EnviaVendaFrete.ZBRSDS0026[] enviaDadosND = new ServicoSAP.EnviaVendaFrete.ZBRSDS0026[] { };
                    ServicoSAP.EnviaVendaFrete.ZBRSDS0025[] enviaDadosNFSe = new ServicoSAP.EnviaVendaFrete.ZBRSDS0025[] { };
                    ServicoSAP.EnviaVendaFrete.ZBRSDS0020[] retornaResultados = new ServicoSAP.EnviaVendaFrete.ZBRSDS0020[] { };
                    ServicoSAP.EnviaVendaFrete.ZBRSDS0003[] t_mensagens = new ServicoSAP.EnviaVendaFrete.ZBRSDS0003[] { };
                    WSSAPNSFE.VendaFrete.ZBRSDS0029[] enviadadosNFSE29 = new WSSAPNSFE.VendaFrete.ZBRSDS0029[] { };
                    WSSAPNSFE.VendaFrete.ZBRSDS0030[] enviadadosNFSE30 = new WSSAPNSFE.VendaFrete.ZBRSDS0030[] { };

                    // passo 1 
                    List<Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024> enviaDadosCTe = new List<Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024>();
                    enviaDadosCRT = null;
                    enviaDadosND = null;
                    enviaDadosNFSe = null;
                    retornaResultados = null;
                    t_mensagens = null;
                    enviadadosNFSE29 = null;
                    enviadadosNFSE30 = null;


                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in CTEs)
                    {
                        if (cte.ModeloDocumentoFiscal.Codigo == 5)
                        {
                            Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024 requestEnviaVendaFrete = PreencherRequestEnviaVendaFrete(cte, cargaIntegracaoPendente.Carga.Operador);
                            enviaDadosCTe.Add(requestEnviaVendaFrete);
                            clientVendaFrete.SI_Envia_Venda_Frete_Sync_OUTAsync(enviaDadosCRT, enviaDadosCTe.ToArray(), enviaDadosND, enviaDadosNFSe, retornaResultados, t_mensagens);

                            servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, inspectorVendaFrete.LastRequestXML, inspectorVendaFrete.LastResponseXML, "xml", $"Envio Dados CTe ({cte.Numero})");

                            enviaDadosCTe.Clear();
                        }
                        else if (cte.ModeloDocumentoFiscal.Codigo == 30)
                        {
                            WSSAPNSFE.VendaFrete.ZBRSDS0029 requestVendaFrete = PreencherRequestVendaFrete(cte, cargaIntegracaoPendente.Carga.Operador);
                            enviadadosNFSE29 = new WSSAPNSFE.VendaFrete.ZBRSDS0029[] {
                                requestVendaFrete
                            };

                            clientVendaFreteNFSe.SI_Venda_Servico_Sync_OUTAsync(enviadadosNFSE29, enviadadosNFSE30);

                            servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, inspectorVendaFreteNFSE.LastRequestXML, inspectorVendaFreteNFSE.LastResponseXML, "xml", $"Envio Dados NFSe ({cte.Numero})");

                            enviaDadosCTe.Clear();
                        }

                    }

                    if (sucesso)
                    {
                        cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                        cargaIntegracaoPendente.Protocolo = "Passo 1 OK";
                        cargaIntegracaoPendente.DataIntegracao = DateTime.Now.AddMinutes(5);
                        cargaIntegracaoPendente.NumeroTentativas = 0;
                    }
                    else
                    {
                        cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracaoPendente.ProblemaIntegracao = mensagemRetorno;
                        cargaIntegracaoPendente.Protocolo = "";
                    }


                    #endregion
                }
                else
                {
                    #region Passo 2 
                    // passo 2 5 minutos depois 3 tentativas de consulta senão erro 

                    Servicos.ServicoSAP.ConsultaCTe.SI_Consulta_Doc_Sync_I101_OUTClient clientConsultaCTe = ObterClientConsultaCTe(configuracaoIntegracao.URLConsultaDocumentos, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                    InspectorBehavior inspectorConsultaCTe = new InspectorBehavior();
                    clientConsultaCTe.Endpoint.EndpointBehaviors.Add(inspectorConsultaCTe);
                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in CTEs)
                    {
                        if (string.IsNullOrEmpty(cte.CodigoEscrituracao) && string.IsNullOrEmpty(cte.CodigoIntegracao) && cte.ModeloDocumentoFiscal.Codigo == 5)
                        {
                            Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Req requisicaoConsulta = PreencherRequestConsultaCTe(cte);
                            Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Resp responseConsulta = clientConsultaCTe.SI_Consulta_Doc_Sync_OUT(requisicaoConsulta);

                            if (!string.IsNullOrWhiteSpace(responseConsulta.MENSAGEM))
                                mensagemRetorno = responseConsulta.MENSAGEM;

                            if (responseConsulta.STATUS_PROC != "S" && responseConsulta.STATUS_ENV != "S")
                                sucesso = false;

                            if (sucesso && !string.IsNullOrEmpty(responseConsulta.DOC_FATURA))
                                AtualizarDadosCTE(cte, responseConsulta.DOC_FATURA, responseConsulta.DOC_VENDA);

                            servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, inspectorConsultaCTe.LastRequestXML, inspectorConsultaCTe.LastResponseXML, "xml", $"Consulta CTe {cte.Numero}  Doc.Fat {responseConsulta?.DOC_FATURA ?? ""}  {mensagemRetorno}");
                        }
                        else if (string.IsNullOrEmpty(cte.CodigoEscrituracao) && string.IsNullOrEmpty(cte.CodigoIntegracao) && cte.ModeloDocumentoFiscal.Codigo == 30)
                        {
                            Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Req requisicaoConsulta = PreencherRequestConsultaCTe(cte);
                            Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Resp responseConsulta = clientConsultaCTe.SI_Consulta_Doc_Sync_OUT(requisicaoConsulta);

                            if (!string.IsNullOrWhiteSpace(responseConsulta.MENSAGEM))
                                mensagemRetorno = responseConsulta.MENSAGEM;

                            if (responseConsulta.STATUS_PROC != "S" && responseConsulta.STATUS_ENV != "S")
                                sucesso = false;

                            if (sucesso && !string.IsNullOrEmpty(responseConsulta.DOC_FATURA))
                                AtualizarDadosCTE(cte, responseConsulta.DOC_FATURA, responseConsulta.DOC_VENDA);

                            servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, inspectorConsultaCTe.LastRequestXML, inspectorConsultaCTe.LastResponseXML, "xml", $"Consulta NFSe {cte.Numero}  Doc.Fat {responseConsulta?.DOC_FATURA ?? ""}  {mensagemRetorno}");
                        }

                    }

                    if (sucesso)
                    {
                        cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaIntegracaoPendente.ProblemaIntegracao = "Integrado com sucesso";
                        cargaIntegracaoPendente.Protocolo = "Passo Final OK";
                    }
                    else
                    {
                        cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracaoPendente.ProblemaIntegracao = mensagemRetorno;
                    }
                    #endregion
                }
            }
            catch (ServicoException excecao)
            {
                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSAP");

                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar com SAP.";
            }

            repCargaIntegracao.Atualizar(cargaIntegracaoPendente);

        }

        public void IntegrarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);

            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            string mensagemRetorno = string.Empty;
            string mensagemRetornoArquivo = string.Empty;

            InspectorBehavior inspectorArmazena = new InspectorBehavior();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoSAP();
                Servicos.ServicoSAP.ArmazenaCTe.SI_Armazena_Cte_Async_OUTClient clientArmazenaCTe = ObterClientArmazenaCTe(configuracaoIntegracao.URL, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);

                clientArmazenaCTe.Endpoint.EndpointBehaviors.Add(inspectorArmazena);

                Servicos.ServicoSAP.ArmazenaCTe.DT_Armazena_Cte_req request = PreencherRequest(integracao.CargaCTe.CTe?.Codigo ?? 0);
                Servicos.ServicoSAP.ArmazenaCTe.SI_Armazena_Cte_Async_OUTResponse response = clientArmazenaCTe.SI_Armazena_Cte_Async_OUTAsync(request).Result;

                if (!string.IsNullOrWhiteSpace(response.MT_Armazena_CTe_resp.T_RETORN.MENSAGEM))
                    mensagemRetornoArquivo = response.MT_Armazena_CTe_resp.T_RETORN.MENSAGEM;

                AdicionarArquivoTransacao(integracao, inspectorArmazena, mensagemRetornoArquivo);

                Servicos.ServicoSAP.EnviaVendaFrete.SI_Envia_Venda_Frete_Sync_OUTClient clientVendaFrete = ObterClientVendaFrete(configuracaoIntegracao.URLEnviaVendaFrete, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                InspectorBehavior inspectorVendaFrete = new InspectorBehavior();
                clientVendaFrete.Endpoint.EndpointBehaviors.Add(inspectorVendaFrete);

                ServicoSAP.EnviaVendaFrete.ZBRSDS0027[] enviaDadosCRT = new ServicoSAP.EnviaVendaFrete.ZBRSDS0027[] { };
                ServicoSAP.EnviaVendaFrete.ZBRSDS0026[] enviaDadosND = new ServicoSAP.EnviaVendaFrete.ZBRSDS0026[] { };
                ServicoSAP.EnviaVendaFrete.ZBRSDS0025[] enviaDadosNFSe = new ServicoSAP.EnviaVendaFrete.ZBRSDS0025[] { };
                ServicoSAP.EnviaVendaFrete.ZBRSDS0020[] retornaResultados = new ServicoSAP.EnviaVendaFrete.ZBRSDS0020[] { };
                ServicoSAP.EnviaVendaFrete.ZBRSDS0003[] t_mensagens = new ServicoSAP.EnviaVendaFrete.ZBRSDS0003[] { };

                Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024 requestEnviaVendaFrete = PreencherRequestEnviaVendaFrete(integracao.CargaCTe.CTe, integracao.CargaOcorrencia.Usuario);
                Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024[] enviaDadosCTe = new Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024[] { requestEnviaVendaFrete };

                clientVendaFrete.SI_Envia_Venda_Frete_Sync_OUTAsync(enviaDadosCRT, enviaDadosCTe, enviaDadosND, enviaDadosNFSe, retornaResultados, t_mensagens);

                AdicionarArquivoTransacao(integracao, inspectorVendaFrete, "Envio Dados CTE");

                Servicos.ServicoSAP.ConsultaCTe.SI_Consulta_Doc_Sync_I101_OUTClient clientConsultaCTe = ObterClientConsultaCTe(configuracaoIntegracao.URLConsultaDocumentos, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                InspectorBehavior inspectorConsultaCTe = new InspectorBehavior();
                clientConsultaCTe.Endpoint.EndpointBehaviors.Add(inspectorConsultaCTe);

                Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Req requisicaoConsulta = PreencherRequestConsultaCTe(integracao.CargaCTe.CTe);
                Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Resp responseConsulta = clientConsultaCTe.SI_Consulta_Doc_Sync_OUT(requisicaoConsulta);

                if (!string.IsNullOrWhiteSpace(responseConsulta.MENSAGEM))
                    mensagemRetornoArquivo = responseConsulta.MENSAGEM;

                AdicionarArquivoTransacao(integracao, inspectorConsultaCTe, mensagemRetornoArquivo);

                if (responseConsulta.STATUS_PROC == "S" && responseConsulta.STATUS_ENV == "S")
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    mensagemRetorno = "Integrado com sucesso";
                }
                else
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    mensagemRetorno = mensagemRetornoArquivo;
                }
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = "Problema ao integrar com o SAP.";
            }

            integracao.ProblemaIntegracao = mensagemRetorno;
            repOcorrenciaCTeIntegracao.Atualizar(integracao);
        }

        public void IntegrarCTE(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao integracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repIntegracaoCte = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);

            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            string mensagemRetorno = string.Empty;
            string mensagemRetornoArquivo = string.Empty;

            InspectorBehavior inspectorArmazena = new InspectorBehavior();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoSAP();
                Servicos.ServicoSAP.ArmazenaCTe.SI_Armazena_Cte_Async_OUTClient clientArmazenaCTe = ObterClientArmazenaCTe(configuracaoIntegracao.URL, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);

                clientArmazenaCTe.Endpoint.EndpointBehaviors.Add(inspectorArmazena);

                Servicos.ServicoSAP.ArmazenaCTe.DT_Armazena_Cte_req request = PreencherRequest(integracao.CTe.Codigo);
                Servicos.ServicoSAP.ArmazenaCTe.SI_Armazena_Cte_Async_OUTResponse response = clientArmazenaCTe.SI_Armazena_Cte_Async_OUTAsync(request).Result;

                if (!string.IsNullOrWhiteSpace(response.MT_Armazena_CTe_resp.T_RETORN.MENSAGEM))
                    mensagemRetornoArquivo = response.MT_Armazena_CTe_resp.T_RETORN.MENSAGEM;

                AdicionarArquivoTransacao(integracao, inspectorArmazena, mensagemRetornoArquivo);

                Servicos.ServicoSAP.EnviaVendaFrete.SI_Envia_Venda_Frete_Sync_OUTClient clientVendaFrete = ObterClientVendaFrete(configuracaoIntegracao.URLEnviaVendaFrete, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                InspectorBehavior inspectorVendaFrete = new InspectorBehavior();
                clientVendaFrete.Endpoint.EndpointBehaviors.Add(inspectorVendaFrete);

                ServicoSAP.EnviaVendaFrete.ZBRSDS0027[] enviaDadosCRT = new ServicoSAP.EnviaVendaFrete.ZBRSDS0027[] { };
                ServicoSAP.EnviaVendaFrete.ZBRSDS0026[] enviaDadosND = new ServicoSAP.EnviaVendaFrete.ZBRSDS0026[] { };
                ServicoSAP.EnviaVendaFrete.ZBRSDS0025[] enviaDadosNFSe = new ServicoSAP.EnviaVendaFrete.ZBRSDS0025[] { };
                ServicoSAP.EnviaVendaFrete.ZBRSDS0020[] retornaResultados = new ServicoSAP.EnviaVendaFrete.ZBRSDS0020[] { };
                ServicoSAP.EnviaVendaFrete.ZBRSDS0003[] t_mensagens = new ServicoSAP.EnviaVendaFrete.ZBRSDS0003[] { };

                Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024 requestEnviaVendaFrete = PreencherRequestEnviaVendaFrete(integracao.CTe, integracao.CTe.Usuario);
                Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024[] enviaDadosCTe = new Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024[] { requestEnviaVendaFrete };

                clientVendaFrete.SI_Envia_Venda_Frete_Sync_OUTAsync(enviaDadosCRT, enviaDadosCTe, enviaDadosND, enviaDadosNFSe, retornaResultados, t_mensagens);

                AdicionarArquivoTransacao(integracao, inspectorVendaFrete, "Envio Dados CTE");

                Servicos.ServicoSAP.ConsultaCTe.SI_Consulta_Doc_Sync_I101_OUTClient clientConsultaCTe = ObterClientConsultaCTe(configuracaoIntegracao.URLConsultaDocumentos, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                InspectorBehavior inspectorConsultaCTe = new InspectorBehavior();
                clientConsultaCTe.Endpoint.EndpointBehaviors.Add(inspectorConsultaCTe);

                Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Req requisicaoConsulta = PreencherRequestConsultaCTe(integracao.CTe);
                Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Resp responseConsulta = clientConsultaCTe.SI_Consulta_Doc_Sync_OUT(requisicaoConsulta);

                if (!string.IsNullOrWhiteSpace(responseConsulta.MENSAGEM))
                    mensagemRetornoArquivo = responseConsulta.MENSAGEM;

                AdicionarArquivoTransacao(integracao, inspectorConsultaCTe, mensagemRetornoArquivo);

                if (responseConsulta.STATUS_PROC == "S" && responseConsulta.STATUS_ENV == "S")
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    mensagemRetorno = "Integrado com sucesso";
                    AtualizarDadosCTE(integracao.CTe, responseConsulta.DOC_FATURA, responseConsulta.DOC_VENDA);
                }
                else
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    mensagemRetorno = mensagemRetornoArquivo;
                }
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = "Problema ao integrar com o SAP.";
            }

            integracao.ProblemaIntegracao = mensagemRetorno;
            repIntegracaoCte.Atualizar(integracao);
        }

        public void IntegrarNFSE(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao integracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repIntegracaoCte = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);

            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            string mensagemRetorno = string.Empty;
            string mensagemRetornoArquivo = string.Empty;

            InspectorBehavior inspectorArmazena = new InspectorBehavior();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoSAP();
                Servicos.ServicoSAP.ArmazenaCTe.SI_Armazena_Cte_Async_OUTClient clientArmazenaCTe = ObterClientArmazenaCTe(configuracaoIntegracao.URL, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);

                clientArmazenaCTe.Endpoint.EndpointBehaviors.Add(inspectorArmazena);

                Servicos.ServicoSAP.ArmazenaCTe.DT_Armazena_Cte_req request = PreencherRequestNFSe(integracao.CTe.Codigo);
                Servicos.ServicoSAP.ArmazenaCTe.SI_Armazena_Cte_Async_OUTResponse response = clientArmazenaCTe.SI_Armazena_Cte_Async_OUTAsync(request).Result;

                if (!string.IsNullOrWhiteSpace(response.MT_Armazena_CTe_resp.T_RETORN.MENSAGEM))
                    mensagemRetornoArquivo = response.MT_Armazena_CTe_resp.T_RETORN.MENSAGEM;

                AdicionarArquivoTransacao(integracao, inspectorArmazena, mensagemRetornoArquivo);

                WSSAPNSFE.VendaFrete.SI_Venda_Servico_Sync_OUTClient clientVendaFretenfse = ObterClientVendaFreteNFSE(configuracaoIntegracao.URLEnviaVendaFrete, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                InspectorBehavior inspectorVendaFrete = new InspectorBehavior();
                clientVendaFretenfse.Endpoint.EndpointBehaviors.Add(inspectorVendaFrete);

                WSSAPNSFE.VendaFrete.ZBRSDS0030[] enviadadosnfse30 = new WSSAPNSFE.VendaFrete.ZBRSDS0030[] { };

                WSSAPNSFE.VendaFrete.ZBRSDS0029 requestEnviaVendaFrete = PreencherRequestVendaFrete(integracao.CTe, integracao.CTe.Usuario);
                WSSAPNSFE.VendaFrete.ZBRSDS0029[] enviadadosnfse29 = new WSSAPNSFE.VendaFrete.ZBRSDS0029[] { requestEnviaVendaFrete };

                clientVendaFretenfse.SI_Venda_Servico_Sync_OUTAsync(enviadadosnfse29, enviadadosnfse30);

                AdicionarArquivoTransacao(integracao, inspectorVendaFrete, "Envio Dados NFSE");

                Servicos.ServicoSAP.ConsultaCTe.SI_Consulta_Doc_Sync_I101_OUTClient clientConsultaCTe = ObterClientConsultaCTe(configuracaoIntegracao.URLConsultaDocumentos, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                InspectorBehavior inspectorConsultaCTe = new InspectorBehavior();
                clientConsultaCTe.Endpoint.EndpointBehaviors.Add(inspectorConsultaCTe);

                Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Req requisicaoConsulta = PreencherRequestConsultaCTe(integracao.CTe);
                Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Resp responseConsulta = clientConsultaCTe.SI_Consulta_Doc_Sync_OUT(requisicaoConsulta);

                if (!string.IsNullOrWhiteSpace(responseConsulta.MENSAGEM))
                    mensagemRetornoArquivo = responseConsulta.MENSAGEM;

                AdicionarArquivoTransacao(integracao, inspectorConsultaCTe, mensagemRetornoArquivo);

                if (responseConsulta.STATUS_PROC == "S" && responseConsulta.STATUS_ENV == "S")
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    mensagemRetorno = "Integrado com sucesso";
                    AtualizarDadosCTE(integracao.CTe, responseConsulta.DOC_FATURA, responseConsulta.DOC_VENDA);
                }
                else
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    mensagemRetorno = mensagemRetornoArquivo;
                }
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = "Problema ao integrar com o SAP.";
            }

            integracao.ProblemaIntegracao = mensagemRetorno;
            repIntegracaoCte.Atualizar(integracao);
        }

        public void IntegrarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSAP repConfiguracaoSAP = new Repositorio.Embarcador.Configuracoes.IntegracaoSAP(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP configuracaoSAP = repConfiguracaoSAP.BuscarPrimeiroRegistro();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarCTePorFatura(faturaIntegracao.Fatura.Codigo);
            InspectorBehavior inspector = new InspectorBehavior();

            faturaIntegracao.DataEnvio = DateTime.Now;
            string mensagemRetorno = "";

            if (faturaIntegracao.SituacaoIntegracao == SituacaoIntegracao.AgRetorno)
            {
                ConsultaIntegracaoSAP(faturaIntegracao, configuracaoSAP, ctes);
            }
            else
            {
                try
                {
                    if (!configuracaoSAP.PossuiIntegracao || !configuracaoSAP.RealizarIntegracaoComDadosFatura)
                        throw new ServicoException("Integração com SAP não está habilitada");

                    if (string.IsNullOrWhiteSpace(configuracaoSAP.URLIntegracaoFatura))
                        throw new ServicoException("Não há URL configurada para a integração com SAP");

                    if (string.IsNullOrWhiteSpace(configuracaoSAP.Usuario) || string.IsNullOrWhiteSpace(configuracaoSAP.Senha))
                        throw new ServicoException("Usuário e senha não configurados para integração SAP");

                    ServicoSAP.AgruparFatura.SI_Agrupar_Fatura_ASync_OUTClient client = ObterClienteIntegracaoFatura(configuracaoSAP);

                    client.Endpoint.EndpointBehaviors.Add(inspector);

                    ServicoSAP.AgruparFatura.DT_Agrupar_Fatura_reqI_DOC[] request = PreencherRequisicaoIntegracaoFatura(faturaIntegracao.Fatura, _unitOfWork, ctes);

                    client.Operation2(request);
                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                    mensagemRetorno = "Enviado arquivo de integração com sucesso";
                }
                catch (ServicoException excecao)
                {
                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    mensagemRetorno = excecao.Message;
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);

                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    mensagemRetorno = "Problema ao integrar com o SAP";
                }

                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork),
                    Data = DateTime.Now,
                    Mensagem = mensagemRetorno,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
                };

                repFaturaIntegracaoArquivo.Inserir(arquivoIntegracao);

                if (faturaIntegracao.ArquivosIntegracao == null)
                    faturaIntegracao.ArquivosIntegracao = new List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo>();
                faturaIntegracao.ArquivosIntegracao.Add(arquivoIntegracao);

                faturaIntegracao.MensagemRetorno = mensagemRetorno;

                if (faturaIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                    faturaIntegracao.Tentativas++;
                else
                    faturaIntegracao.Tentativas = 0;
                repFaturaIntegracao.Atualizar(faturaIntegracao);
            }
            _unitOfWork.CommitChanges();
        }

        public void IntegrarEstornoFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSAP repConfiguracaoSAP = new Repositorio.Embarcador.Configuracoes.IntegracaoSAP(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP configuracaoSAP = repConfiguracaoSAP.BuscarPrimeiroRegistro();

            ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarCTePorFatura(faturaIntegracao.Fatura.Codigo);

            InspectorBehavior inspector = new InspectorBehavior();

            faturaIntegracao.DataEnvio = DateTime.Now;
            string mensagemRetorno = "";

            if (faturaIntegracao.SituacaoIntegracao == SituacaoIntegracao.AgRetorno)
            {
                ConsultaIntegracaoSAP(faturaIntegracao, configuracaoSAP, ctes);
            }
            else
            {
                try
                {
                    if (!configuracaoSAP.PossuiIntegracao || !configuracaoSAP.RealizarIntegracaoComDadosFatura)
                        throw new ServicoException("Integração com SAP não está habilitada");

                    if (string.IsNullOrWhiteSpace(configuracaoSAP.URLIntegracaoFatura))
                        throw new ServicoException("Não há URL configurada para a integração com SAP");

                    if (string.IsNullOrWhiteSpace(configuracaoSAP.Usuario) || string.IsNullOrWhiteSpace(configuracaoSAP.Senha))
                        throw new ServicoException("Usuário e senha não configurados para integração SAP");


                    Servicos.ServicoSAP.EstornaFatura.SI_Estorno_Agrup_AsyncClient client = ObterClienteIntegracaoEstornoFatura(configuracaoSAP);

                    client.Endpoint.EndpointBehaviors.Add(inspector);

                    Servicos.ServicoSAP.EstornaFatura.DT_Estorno_Agrup_Async request = PreencherRequisicaoIntegracaoEstornoFatura(faturaIntegracao.Fatura, _unitOfWork, ctes);

                    client.SI_Estorno_Agrup_Async(request);

                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                    mensagemRetorno = "Enviado solicitãção de estorno.";
                }
                catch (ServicoException excecao)
                {
                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    mensagemRetorno = excecao.Message;
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);

                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    mensagemRetorno = "Problema ao integrar com o SAP";
                }

                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork),
                    Data = DateTime.Now,
                    Mensagem = mensagemRetorno,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
                };

                repFaturaIntegracaoArquivo.Inserir(arquivoIntegracao);

                if (faturaIntegracao.ArquivosIntegracao == null)
                    faturaIntegracao.ArquivosIntegracao = new List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo>();
                faturaIntegracao.ArquivosIntegracao.Add(arquivoIntegracao);

                faturaIntegracao.MensagemRetorno = mensagemRetorno;

                if (faturaIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                    faturaIntegracao.Tentativas++;
                else
                    faturaIntegracao.Tentativas = 0;
                repFaturaIntegracao.Atualizar(faturaIntegracao);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConsultaAgrupamento_I067 ObterDadosConsultaAgrupamento_I067(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.ConhecimentoDeTransporteEletronico ctes)
        {
            Guid guid = Guid.NewGuid();
            Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConsultaAgrupamento_I067 consulta = new Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConsultaAgrupamento_I067
            {
                GUID = guid.ToString(),
                Origem = "0022",
                Cliente = fatura.ClienteTomadorFatura?.CodigoIntegracao ?? string.Empty,
                Referencia = ctes?.Numero.ToString() ?? string.Empty,
                IdControle = "MULTI" + (fatura.Codigo.ToString() ?? string.Empty),
                NumFatura = ctes?.CodigoEscrituracao?.ToString() ?? string.Empty
            };
            return consulta;
        }

        public void ConsultaIntegracaoSAP(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP configuracaoSAP, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);

            ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string requestBody = "";
            string responseBody = "";

            List<string> lstRequestBody = new List<string>();
            List<string> lstresponseBody = new List<string>();


            try
            {
                if (string.IsNullOrEmpty(configuracaoSAP?.URLConsultaFatura ?? ""))
                    throw new ServicoException("URL Consulta FaturaIntegração invalida.");

                foreach (var cte in ctes)
                {
                    bool sair = false;


                    string url = configuracaoSAP.URLConsultaFatura;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    var client = new RestClient(url);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    string encodedCredentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{configuracaoSAP.Usuario}:{configuracaoSAP.Senha}"));
                    request.AddHeader("Authorization", "Basic " + encodedCredentials);
                    request.AddHeader("Content-Type", "application/json");

                    Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConsultaAgrupamento_I067 consulta = ObterDadosConsultaAgrupamento_I067(faturaIntegracao.Fatura, cte);
                    request.AddJsonBody(consulta);
                    IRestResponse response = client.Execute(request);

                    if (response?.ErrorException != null)
                        throw new Exception($"Ocorreu um erro ao acesso o webservice: {response?.ErrorException?.Message}");

                    requestBody = request.Body.Value.ToString();
                    responseBody = response.Content;

                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    faturaIntegracao.MensagemRetorno = "Fatura confirmada com sucesso.";

                    lstRequestBody.Add(requestBody);
                    lstresponseBody.Add(responseBody);

                    try
                    {
                        if (retorno.Agrups.Agrup.Count == null)
                        {
                            for (int j = 0; j < retorno.Agrups.Agrup.Item.Itens.Msg.Msgs.Count; j++)
                            {
                                if (retorno.Agrups.Agrup.Item.Itens.Msg.Msgs[j].Type.ToString() != "S" && retorno.Agrups.Agrup.Item.Itens.Msg.Msgs[j].Type.ToString() != "D" && retorno.Agrups.Agrup.Item.Itens.Msg.Msgs[j].Type.ToString() != "R")
                                {
                                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                                    faturaIntegracao.MensagemRetorno = $"Erro na consulta {retorno.Agrups.Agrup.Item.Itens.Msg.Msgs[j].Message.ToString()}";
                                    sair = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < retorno.Agrups.Agrup.Count; i++)
                            {
                                for (int j = 0; j < retorno.Agrups.Agrup[i].Item.Itens.Msg.Msgs.Count; j++)
                                {
                                    if (retorno.Agrups.Agrup[i].Item.Itens.Msg.Msgs[j].Type.ToString() != "S" && retorno.Agrups.Agrup[i].Item.Itens.Msg.Msgs[j].Type.ToString() != "D" && retorno.Agrups.Agrup[i].Item.Itens.Msg.Msgs[j].Type.ToString() != "R")
                                    {
                                        faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                                        faturaIntegracao.MensagemRetorno = $"Erro na consulta {retorno.Agrups.Agrup[i].Item.Itens.Msg.Msgs[j].Message.ToString()}";
                                        sair = true;
                                        break;
                                    }
                                }
                                if (sair)
                                    break;
                            }
                        }
                    }
                    catch (Exception)
                    { // feito assim pq existem 2 objetos de retorno diferentes 
                        dynamic erro = JsonConvert.DeserializeObject<dynamic>(response.Content);
                        faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                        faturaIntegracao.MensagemRetorno = $"Erro na consulta: {erro.IntStMessage.ToString()}.";
                    }
                    if (sair)
                        break;

                }

                for (int i = 0; i < lstRequestBody.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo
                    {
                        ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(lstRequestBody[i], "txt", _unitOfWork),
                        ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(lstresponseBody[i], "txt", _unitOfWork),
                        Data = DateTime.Now,
                        Mensagem = faturaIntegracao.MensagemRetorno,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
                    };

                    repFaturaIntegracaoArquivo.Inserir(arquivoIntegracao);

                    if (faturaIntegracao.ArquivosIntegracao == null)
                        faturaIntegracao.ArquivosIntegracao = new List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo>();
                    faturaIntegracao.ArquivosIntegracao.Add(arquivoIntegracao);
                }

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                faturaIntegracao.MensagemRetorno = $"Erro na consulta: {excecao.Message}";
            }


            faturaIntegracao.Tentativas++;
            repFaturaIntegracao.Atualizar(faturaIntegracao);
        }

        public bool IntegrarCIOTAcrescimosDescontos(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato, out string mensagemRetorno)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo();

            mensagemRetorno = string.Empty;
            integracaoContrato.DataIntegracao = DateTime.Now;
            integracaoContrato.NumeroTentativas += 1;

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoSAP();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLDescontoAvaria))
                    throw new ServicoException("Não há URL configurada para a integração com SAP");

                if (string.IsNullOrWhiteSpace(configuracaoIntegracao.Usuario) || string.IsNullOrWhiteSpace(configuracaoIntegracao.Senha))
                    throw new ServicoException("Usuário e senha não configurados para integração SAP");

                Servicos.ServicoSAP.AV.SI_Desconto_Avaria_Sync_OUTClient clientDescontoAvaria = ObterClientDescontoAvaria(configuracaoIntegracao.URL, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                clientDescontoAvaria.Endpoint.EndpointBehaviors.Add(inspector);

                Servicos.ServicoSAP.AV.DT_Desconto_Avaria_req request = PreencherRequestDescontoAvaria(integracaoContrato);
                Servicos.ServicoSAP.AV.DT_Desconto_Avaria_resp response = clientDescontoAvaria.SI_Desconto_Avaria_Sync_OUT(request);

                if (response.T_RETURN[0].TEXTO_MSG != "S")
                {
                    integracaoContrato.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    mensagemRetorno = response.T_RETURN[0].TEXTO_MSG;
                }
                else
                {
                    integracaoContrato.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    mensagemRetorno = $"Integrado com sucesso! {response.T_RETURN[0].TEXTO_MSG}";
                }
            }
            catch (ServicoException excecao)
            {
                mensagemRetorno = excecao.Message;
                return false;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                mensagemRetorno = $"Problema ao integrar com o SAP - AV: {excecao.Message}";
                integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.ArquivosTransacao.Add(AdicionarArquivoIntegracao(integracaoArquivo, inspector, mensagemRetorno));

                return false;
            }
            integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.ArquivosTransacao.Add(AdicionarArquivoIntegracao(integracaoArquivo, inspector, mensagemRetorno));

            return true;
        }

        public void IntegrarEncerramentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao integracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao(_unitOfWork);

            string mensagemRetorno;

            integracao.DataIntegracao = DateTime.Now;
            integracao.NumeroTentativas += 1;

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoSAP();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLCriarSaldoFrete))
                    throw new ServicoException("Não há URL configurada para a integração com SAP - SU");

                Servicos.ServicoSAP_SU.SI_Criar_Saldo_Frete_Sync_OUTClient clientCriarSaldoFrete = ObterClientCriarSaldoFrete(configuracaoIntegracao.URLCriarSaldoFrete, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                clientCriarSaldoFrete.Endpoint.EndpointBehaviors.Add(inspector);

                Servicos.ServicoSAP_SU.DT_Criar_Saldo_Frete_req request = PreencherRequisicaoCriarSaldoFrete(integracao);
                Servicos.ServicoSAP_SU.DT_Criar_Saldo_Frete_resp response = clientCriarSaldoFrete.SI_Criar_Saldo_Frete_Sync_OUT(request);

                if (response.T_RETURN[0].TIPO != "S")
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    mensagemRetorno = response.T_RETURN[0].TEXTO_MSG;
                }
                else
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    mensagemRetorno = $"Integrado com sucesso! {response.T_RETURN[0].TEXTO_MSG}";
                }
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = $"Problema ao integrar com o SAP - SU: {excecao.Message}";
            }

            servicoArquivoTransacao.Adicionar(integracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagemRetorno);

            integracao.ProblemaIntegracao = mensagemRetorno;
            repIntegracao.Atualizar(integracao);
        }

        public void IntegrarDadosCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento integracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento repIntegracaoDadosCancelamento = new Repositorio.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento(_unitOfWork);

            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            string mensagemRetorno = string.Empty;

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoSAP();
                Servicos.ServicoSAP.SolicitaCancelamento.SI_Solicita_Cancel_ST_Sync_OUTClient client = ObterClientSolicitacaoCancelamentoCarga(configuracaoIntegracao.URLSolicitacaoCancelamento, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);

                client.Endpoint.EndpointBehaviors.Add(inspector);

                Servicos.ServicoSAP.SolicitaCancelamento.DT_Solicita_Cancel_ST_req request = PreencherRequestSolicitacaoCancelamentoCarga(integracao);
                Servicos.ServicoSAP.SolicitaCancelamento.SI_Solicita_Cancel_ST_Sync_OUTResponse response = client.SI_Solicita_Cancel_ST_Sync_OUTAsync(request).Result;

                servicoArquivoTransacao.Adicionar(inspector.LastRequestXML, inspector.LastResponseXML, ".xml", integracao);

                Servicos.ServicoSAP.ConsultaCTe.SI_Consulta_Doc_Sync_I101_OUTClient clientConsultaCTe = ObterClientConsultaCTe(configuracaoIntegracao.URLConsultaDocumentos, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                InspectorBehavior inspectorConsultaCTe = new InspectorBehavior();
                clientConsultaCTe.Endpoint.EndpointBehaviors.Add(inspectorConsultaCTe);

                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in integracao.CargaCancelamento.Carga.CargaCTes.Select(o => o.CTe).ToList())
                {
                    Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Req requisicaoConsulta = PreencherRequestConsultaCTe(cte);
                    Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Resp responseConsulta = clientConsultaCTe.SI_Consulta_Doc_Sync_OUT(requisicaoConsulta);

                    if (!string.IsNullOrWhiteSpace(responseConsulta.MENSAGEM))
                        mensagemRetorno = responseConsulta.MENSAGEM;

                    servicoArquivoTransacao.Adicionar(integracao, inspectorConsultaCTe.LastRequestXML, inspectorConsultaCTe.LastResponseXML, "xml", $"Consulta CTe {cte.Numero}  Doc.Fat {responseConsulta?.DOC_FATURA ?? ""}  {mensagemRetorno}");
                }

                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = "Problema ao integrar com o SAP.";
            }

            servicoArquivoTransacao.Adicionar(inspector.LastRequestXML, inspector.LastResponseXML, ".xml", integracao);

            integracao.ProblemaIntegracao = mensagemRetorno;
            repIntegracaoDadosCancelamento.Atualizar(integracao);
        }

        public void IntegrarDadosCancelamentoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados integracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados repIntegracaoDadosCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados(_unitOfWork);

            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            string mensagemRetorno = string.Empty;

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoSAP();
                Servicos.ServicoSAP.SolicitaCancelamentoCTe.SI_Solicita_Cancelamento_Sync_OUTClient client = ObterClientSolicitacaoCancelamentoCTe(configuracaoIntegracao.URLSolicitacaoCancelamentoCTe, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);

                client.Endpoint.EndpointBehaviors.Add(inspector);

                Servicos.ServicoSAP.SolicitaCancelamentoCTe.DT_Solicita_Cancelamento_req request = PreencherRequestSolicitacaoCancelamentoCTe(integracao);
                Servicos.ServicoSAP.SolicitaCancelamentoCTe.SI_Solicita_Cancelamento_Sync_OUTResponse response = client.SI_Solicita_Cancelamento_Sync_OUTAsync(request).Result;

                servicoArquivoTransacao.Adicionar(inspector.LastRequestXML, inspector.LastResponseXML, ".xml", integracao);

                Servicos.ServicoSAP.ConsultaCTe.SI_Consulta_Doc_Sync_I101_OUTClient clientConsultaCTe = ObterClientConsultaCTe(configuracaoIntegracao.URLConsultaDocumentos, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                InspectorBehavior inspectorConsultaCTe = new InspectorBehavior();
                clientConsultaCTe.Endpoint.EndpointBehaviors.Add(inspectorConsultaCTe);

                Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Req requisicaoConsulta = PreencherRequestConsultaCTe(integracao.CargaCTe.CTe);
                Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Resp responseConsulta = clientConsultaCTe.SI_Consulta_Doc_Sync_OUT(requisicaoConsulta);

                if (!string.IsNullOrWhiteSpace(responseConsulta.MENSAGEM))
                    mensagemRetorno = responseConsulta.MENSAGEM;

                servicoArquivoTransacao.Adicionar(inspectorConsultaCTe.LastRequestXML, inspectorConsultaCTe.LastResponseXML, ".xml", integracao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = "Problema ao integrar com o SAP.";
            }

            integracao.ProblemaIntegracao = mensagemRetorno;
            repIntegracaoDadosCancelamento.Atualizar(integracao);
        }

        public void IntegrarCancelamentoCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao integracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repIntegracaoCte = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(_unitOfWork);

            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            string mensagemRetorno = string.Empty;

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoSAP();
                Servicos.ServicoSAP.EnviaVendaFrete.SI_Envia_Venda_Frete_Sync_OUTClient clientVendaFrete = ObterClientVendaFrete(configuracaoIntegracao.URLEnviaVendaFrete, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);

                clientVendaFrete.Endpoint.EndpointBehaviors.Add(inspector);

                if (integracao.CargaCTe.CTe.ModeloDocumentoFiscal.Codigo == 30)
                {
                    IntegrarCancelamentoCargaNFSe(integracao);
                }
                else
                {
                    ServicoSAP.EnviaVendaFrete.ZBRSDS0027[] enviaDadosCRT = new ServicoSAP.EnviaVendaFrete.ZBRSDS0027[] { };
                    ServicoSAP.EnviaVendaFrete.ZBRSDS0026[] enviaDadosND = new ServicoSAP.EnviaVendaFrete.ZBRSDS0026[] { };
                    ServicoSAP.EnviaVendaFrete.ZBRSDS0025[] enviaDadosNFSe = new ServicoSAP.EnviaVendaFrete.ZBRSDS0025[] { };
                    ServicoSAP.EnviaVendaFrete.ZBRSDS0020[] retornaResultados = new ServicoSAP.EnviaVendaFrete.ZBRSDS0020[] { };
                    ServicoSAP.EnviaVendaFrete.ZBRSDS0003[] t_mensagens = new ServicoSAP.EnviaVendaFrete.ZBRSDS0003[] { };

                    Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024 requestEnviaVendaFrete = PreencherRequestEnviaVendaFrete(integracao.CargaCTe.CTe, integracao.CargaCancelamento.Usuario, true);
                    Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024[] enviaDadosCTe = new Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024[] { requestEnviaVendaFrete };

                    clientVendaFrete.SI_Envia_Venda_Frete_Sync_OUTAsync(enviaDadosCRT, enviaDadosCTe, enviaDadosND, enviaDadosNFSe, retornaResultados, t_mensagens);

                    AdicionarArquivoTransacao(integracao, inspector, mensagemRetorno);

                    Servicos.ServicoSAP.ConsultaCTe.SI_Consulta_Doc_Sync_I101_OUTClient clientConsultaCTe = ObterClientConsultaCTe(configuracaoIntegracao.URLConsultaDocumentos, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                    InspectorBehavior inspectorConsultaCTe = new InspectorBehavior();
                    clientConsultaCTe.Endpoint.EndpointBehaviors.Add(inspectorConsultaCTe);

                    Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Req requisicaoConsulta = PreencherRequestConsultaCTe(integracao.CargaCTe.CTe);
                    Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Resp responseConsulta = clientConsultaCTe.SI_Consulta_Doc_Sync_OUT(requisicaoConsulta);

                    if (!string.IsNullOrWhiteSpace(responseConsulta.MENSAGEM))
                        mensagemRetorno = responseConsulta.MENSAGEM;

                    AdicionarArquivoTransacao(integracao, inspectorConsultaCTe, mensagemRetorno);

                    integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                }


            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = "Problema ao integrar com o SAP.";
            }

            AdicionarArquivoTransacao(integracao, inspector, mensagemRetorno);

            integracao.ProblemaIntegracao = mensagemRetorno;
            repIntegracaoCte.Atualizar(integracao);
        }

        public void IntegrarCancelamentoCargaNFSe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao integracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repIntegracaoCte = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(_unitOfWork);

            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            string mensagemRetorno = string.Empty;

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoSAP();
                WSSAPNSFE.VendaFrete.SI_Venda_Servico_Sync_OUTClient clientVendaFrete = ObterClientVendaFreteNFSE(configuracaoIntegracao.URLEnviaVendaFrete, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);

                clientVendaFrete.Endpoint.EndpointBehaviors.Add(inspector);

                WSSAPNSFE.VendaFrete.ZBRSDS0030[] enviaDadosNfse30 = new WSSAPNSFE.VendaFrete.ZBRSDS0030[] { };

                WSSAPNSFE.VendaFrete.ZBRSDS0029 requestVendaFrete = PreencherRequestVendaFrete(integracao.CargaCTe.CTe, integracao.CargaCancelamento.Usuario, true);
                WSSAPNSFE.VendaFrete.ZBRSDS0029[] enviaDadosNfse29 = new WSSAPNSFE.VendaFrete.ZBRSDS0029[] { requestVendaFrete };

                clientVendaFrete.SI_Venda_Servico_Sync_OUTAsync(enviaDadosNfse29, enviaDadosNfse30);

                AdicionarArquivoTransacao(integracao, inspector, mensagemRetorno);

                Servicos.ServicoSAP.ConsultaCTe.SI_Consulta_Doc_Sync_I101_OUTClient clientConsultaCTe = ObterClientConsultaCTe(configuracaoIntegracao.URLConsultaDocumentos, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                InspectorBehavior inspectorConsultaCTe = new InspectorBehavior();
                clientConsultaCTe.Endpoint.EndpointBehaviors.Add(inspectorConsultaCTe);

                Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Req requisicaoConsulta = PreencherRequestConsultaCTe(integracao.CargaCTe.CTe);
                Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Resp responseConsulta = clientConsultaCTe.SI_Consulta_Doc_Sync_OUT(requisicaoConsulta);

                if (!string.IsNullOrWhiteSpace(responseConsulta.MENSAGEM))
                    mensagemRetorno = responseConsulta.MENSAGEM;

                AdicionarArquivoTransacao(integracao, inspectorConsultaCTe, mensagemRetorno);

                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = "Problema ao integrar Cancelamento NFSE com o SAP.";
            }

            AdicionarArquivoTransacao(integracao, inspector, mensagemRetorno);

            integracao.ProblemaIntegracao = mensagemRetorno;
            repIntegracaoCte.Atualizar(integracao);
        }

        public void IntegrarDadosCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string processo = string.IsNullOrWhiteSpace(cargaDadosTransporteIntegracao?.Protocolo) ? "CRE" : "CHG";
            string protocolo = (cargaDadosTransporteIntegracao?.Protocolo ?? cargaDadosTransporteIntegracao?.Carga?.CodigoCargaEmbarcador) ?? string.Empty;

            string numeroRetornoSAP = IntegrarDadosCargaAPI4(cargaDadosTransporteIntegracao, cargaDadosTransporteIntegracao.Carga, out string jsonRequisicao, out string jsonRetorno, processo, protocolo);

            if (!string.IsNullOrWhiteSpace(numeroRetornoSAP))
            {
                if (string.IsNullOrWhiteSpace(cargaDadosTransporteIntegracao?.Protocolo))
                    cargaDadosTransporteIntegracao.Protocolo = protocolo;

                bool incrementaCodigoCarga = cargaDadosTransporteIntegracao.Carga.TipoOperacao?.ConfiguracaoCarga?.IncrementaCodigoPorTipoOperacao ?? false;
                string codIntegracaoTipoOperacao = cargaDadosTransporteIntegracao.Carga.TipoOperacao?.CodigoIntegracao ?? string.Empty;
                string numeroRetornoSAPConcat = string.Empty;

                if (incrementaCodigoCarga)
                    numeroRetornoSAPConcat = numeroRetornoSAP;
                else
                    numeroRetornoSAPConcat = string.IsNullOrWhiteSpace(codIntegracaoTipoOperacao) ? numeroRetornoSAP : $"{codIntegracaoTipoOperacao}-{numeroRetornoSAP}";

                string numeroCargaAnterior = cargaDadosTransporteIntegracao.Carga.CodigoCargaEmbarcador ?? string.Empty;

                cargaDadosTransporteIntegracao.Carga.CodigoCargaEmbarcador = Utilidades.String.Left(numeroRetornoSAPConcat.Trim(), 50);
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaDadosTransporteIntegracao, $"Alterou número da carga via SAP API 4. Número da Carga: {numeroCargaAnterior} - Alterado para: {numeroRetornoSAPConcat}", _unitOfWork);
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");
            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarDadosCargaCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string protocolo = repCargaDadosTransporteIntegracao.BuscarProtocoloPorCargaETipoIntegracao(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_API4);

            if (string.IsNullOrEmpty(protocolo))
            {
                throw new ServicoException("Não é possivel cancelar a carga pois não existe protocolo");
            }

            string processo = "DEL";
            IntegrarDadosCargaAPI4(cargaCancelamentoCargaIntegracao, cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga, out string jsonRequisicao, out string jsonRetorno, processo, protocolo);

            servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            repositorioCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo AdicionarArquivoIntegracao(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo, InspectorBehavior inspector, string mensagem)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo repContratoFreteIntegracaoArquivo = new Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo(_unitOfWork);

            integracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            integracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);
            integracaoArquivo.Data = DateTime.Now;
            integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
            integracaoArquivo.Mensagem = mensagem;

            repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);

            return integracaoArquivo;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConfiguracaoIntegracao ObterConfiguracaoIntegracaoSAP()
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP integracaoSAP = BuscarConfiguracoesIntegracao();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.ConfiguracaoIntegracao()
            {
                URL = integracaoSAP.URL,
                URLEnviaVendaFrete = integracaoSAP.URLEnviaVendaFrete,
                URLDescontoAvaria = integracaoSAP.URLDescontoAvaria,
                URLCriarSaldoFrete = integracaoSAP.URLCriarSaldoFrete,
                URLConsultaDocumentos = integracaoSAP.URLConsultaDocumentos,
                URLSolicitacaoCancelamento = integracaoSAP.URLSolicitacaoCancelamento,
                URLSolicitacaoCancelamentoCTe = integracaoSAP.URLSolicitacaoCancelamentoCTe,
                Usuario = integracaoSAP.Usuario,
                Senha = integracaoSAP.Senha,
                URLConsultaFatura = integracaoSAP.URLConsultaFatura,
                URLIntegracaoEstornoFatura = integracaoSAP.URLIntegracaoEstornoFatura,
                URLConsultaEstornoFatura = integracaoSAP.URLConsultaEstornoFatura
            };
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP BuscarConfiguracoesIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSAP repositorioIntegracaoSAP = new Repositorio.Embarcador.Configuracoes.IntegracaoSAP(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP integracaoSAP = repositorioIntegracaoSAP.Buscar();

            ValidarConfiguracaoIntegracao(integracaoSAP);

            return integracaoSAP;
        }

        private void ValidarConfiguracaoIntegracao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP integracaoSAP)
        {
            if ((integracaoSAP == null) || !integracaoSAP.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para SAP.");

            if (string.IsNullOrWhiteSpace(integracaoSAP.Usuario) || string.IsNullOrWhiteSpace(integracaoSAP.Senha))
                throw new ServicoException("Usuário e senha devem estar preenchidos na configuração de integração da SAP.");

            if (string.IsNullOrWhiteSpace(integracaoSAP.URL))
                throw new ServicoException("Não há URL configurada para integração com a SAP");
        }

        private static Servicos.ServicoSAP.ArmazenaCTe.SI_Armazena_Cte_Async_OUTClient ObterClientArmazenaCTe(string url, string usuario, string senha)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            Servicos.ServicoSAP.ArmazenaCTe.SI_Armazena_Cte_Async_OUTClient client = new Servicos.ServicoSAP.ArmazenaCTe.SI_Armazena_Cte_Async_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = usuario;
            client.ClientCredentials.UserName.Password = senha;

            return client;
        }

        private static Servicos.ServicoSAP.EnviaVendaFrete.SI_Envia_Venda_Frete_Sync_OUTClient ObterClientVendaFrete(string url, string usuario, string senha)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            Servicos.ServicoSAP.EnviaVendaFrete.SI_Envia_Venda_Frete_Sync_OUTClient client = new Servicos.ServicoSAP.EnviaVendaFrete.SI_Envia_Venda_Frete_Sync_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = usuario;
            client.ClientCredentials.UserName.Password = senha;

            return client;
        }

        private static WSSAPNSFE.VendaFrete.SI_Venda_Servico_Sync_OUTClient ObterClientVendaFreteNFSE(string url, string usuario, string senha)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            WSSAPNSFE.VendaFrete.SI_Venda_Servico_Sync_OUTClient client = new WSSAPNSFE.VendaFrete.SI_Venda_Servico_Sync_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = usuario;
            client.ClientCredentials.UserName.Password = senha;

            return client;
        }

        private Servicos.ServicoSAP.AV.SI_Desconto_Avaria_Sync_OUTClient ObterClientDescontoAvaria(string url, string usuario, string senha)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            Servicos.ServicoSAP.AV.SI_Desconto_Avaria_Sync_OUTClient client = new Servicos.ServicoSAP.AV.SI_Desconto_Avaria_Sync_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = usuario;
            client.ClientCredentials.UserName.Password = senha;

            return client;
        }

        private Servicos.ServicoSAP_SU.SI_Criar_Saldo_Frete_Sync_OUTClient ObterClientCriarSaldoFrete(string url, string usuario, string senha)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            Servicos.ServicoSAP_SU.SI_Criar_Saldo_Frete_Sync_OUTClient client = new Servicos.ServicoSAP_SU.SI_Criar_Saldo_Frete_Sync_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = usuario;
            client.ClientCredentials.UserName.Password = senha;

            return client;
        }

        private static Servicos.ServicoSAP.ConsultaCTe.SI_Consulta_Doc_Sync_I101_OUTClient ObterClientConsultaCTe(string url, string usuario, string senha)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            Servicos.ServicoSAP.ConsultaCTe.SI_Consulta_Doc_Sync_I101_OUTClient client = new Servicos.ServicoSAP.ConsultaCTe.SI_Consulta_Doc_Sync_I101_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = usuario;
            client.ClientCredentials.UserName.Password = senha;

            return client;
        }

        private static Servicos.ServicoSAP.SolicitaCancelamento.SI_Solicita_Cancel_ST_Sync_OUTClient ObterClientSolicitacaoCancelamentoCarga(string url, string usuario, string senha)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            Servicos.ServicoSAP.SolicitaCancelamento.SI_Solicita_Cancel_ST_Sync_OUTClient client = new Servicos.ServicoSAP.SolicitaCancelamento.SI_Solicita_Cancel_ST_Sync_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = usuario;
            client.ClientCredentials.UserName.Password = senha;

            return client;
        }

        private static Servicos.ServicoSAP.SolicitaCancelamentoCTe.SI_Solicita_Cancelamento_Sync_OUTClient ObterClientSolicitacaoCancelamentoCTe(string url, string usuario, string senha)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            Servicos.ServicoSAP.SolicitaCancelamentoCTe.SI_Solicita_Cancelamento_Sync_OUTClient client = new Servicos.ServicoSAP.SolicitaCancelamentoCTe.SI_Solicita_Cancelamento_Sync_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = usuario;
            client.ClientCredentials.UserName.Password = senha;

            return client;
        }

        private Servicos.ServicoSAP.ArmazenaCTe.DT_Armazena_Cte_req PreencherRequest(int codigoCTe)
        {
            Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);

            Repositorio.XMLCTe repositorioXMLCTe = new Repositorio.XMLCTe(_unitOfWork);

            Dominio.Entidades.XMLCTe xmlCTE = repositorioXMLCTe.BuscarPorCTe(codigoCTe, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

            if (xmlCTE == null)
                throw new ServicoException("XML do CTe não encontrado");

            Servicos.ServicoSAP.ArmazenaCTe.DT_Armazena_Cte_req request = new ServicoSAP.ArmazenaCTe.DT_Armazena_Cte_req();

            if (!xmlCTE.XMLArmazenadoEmArquivo)
            {
                request.XML_CTe = xmlCTE.XML;
            }
            else
            {
                string caminho = servicoCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(xmlCTE.CTe, "A", _unitOfWork);

                XmlDocument doc = new XmlDocument();
                doc.Load(caminho);
                string stringXMLCTe = doc.InnerXml;

                stringXMLCTe = stringXMLCTe.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "");

                request.XML_CTe = stringXMLCTe;
            }

            return request;
        }

        private Servicos.ServicoSAP.ArmazenaCTe.DT_Armazena_Cte_req PreencherRequestNFSe(int codigoCTe)
        {
            Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);

            Repositorio.XMLCTe repositorioXMLCTe = new Repositorio.XMLCTe(_unitOfWork);

            Dominio.Entidades.XMLCTe xmlCTE = repositorioXMLCTe.BuscarPorCTe(codigoCTe, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

            if (xmlCTE == null)
                throw new ServicoException("XML da NFSe não encontrado");

            Servicos.ServicoSAP.ArmazenaCTe.DT_Armazena_Cte_req request = new ServicoSAP.ArmazenaCTe.DT_Armazena_Cte_req();

            if (!xmlCTE.XMLArmazenadoEmArquivo)
            {
                request.XML_CTe = xmlCTE.XML;
            }
            else
            {
                string caminho = servicoCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(xmlCTE.CTe, "A", _unitOfWork);

                XmlDocument doc = new XmlDocument();
                doc.Load(caminho);
                string stringXMLCTe = doc.InnerXml;

                stringXMLCTe = stringXMLCTe.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "");

                request.XML_CTe = stringXMLCTe;
            }

            return request;
        }

        private Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024 PreencherRequestEnviaVendaFrete(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Usuario usario, bool cancelamentoCteProtocoloRequest = false)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(cte.Tomador.CPF_CNPJ.ToDouble());
            Dominio.Entidades.Cliente clienteOrigem = repCliente.BuscarPorCPFCNPJ(cte.Expedidor != null ? cte.Expedidor.CPF_CNPJ.ToDouble() : cte.Remetente.CPF_CNPJ.ToDouble());
            Dominio.Entidades.Cliente clienteDestino = repCliente.BuscarPorCPFCNPJ(cte.Recebedor != null ? cte.Recebedor.CPF_CNPJ.ToDouble() : cte.Destinatario.CPF_CNPJ.ToDouble());

            Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024 request = new ServicoSAP.EnviaVendaFrete.ZBRSDS0024();
            request.ORIGEM = "0022";

            // Quanto CT-e Complementar enviar "ZCT5" fixo
            request.TPORDVDA = cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento ? "ZCT5" : "ZCT1";
            request.FLAGREFATURA = string.Empty;
            request.NRCONTRATO = tomador?.CodigoCompanhia;
            request.NRPEDIDOCLI = cte.Numero.ToString() ?? string.Empty;
            request.DTPEDIDOCLI = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("yyyy-MM-dd") : string.Empty;
            request.DATA_EMIS = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("yyyy-MM-dd") : string.Empty;
            request.CONDPAGTO = "Z000";
            request.FORMAPAGO = cte.CondicaoPagamento2;
            request.SAPSERVICO = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? "5200000027" : "000000005100000059";
            request.CODCFOP = (cte.CFOP != null && cte.CFOP.CFOPComExtensao.EndsWith("AA")) ? cte.CFOP.CFOPComExtensao : $"{cte.CFOP.CFOPComExtensao}AA";
            request.CLUCRO = cte.CentroResultadoFaturamento?.PlanoContabilidade ?? string.Empty;
            request.SAPPOSITEM = "0";
            request.FILIALCENTRO = cte.Empresa?.CNPJ ?? string.Empty;
            request.SAPQTDE = 1;
            request.SAPQTDESpecified = true;
            request.PRECOBRUTO1 = Math.Round(cte.ValorPrestacaoServico, 2);
            request.PRECOBRUTO1Specified = true;
            request.PRECOBRUTO2 = Math.Round(cte.ValorPrestacaoServico, 2);
            request.PRECOBRUTO2Specified = true;
            SetarValoresICMs(ref request, cte);
            SetarValoresPISCOFINS(ref request, cte);
            request.AGPARCEIRO = clienteOrigem?.CodigoIntegracao;
            request.WEPARCEIRO = clienteDestino?.CodigoIntegracao;
            request.REPARCEIRO = tomador?.CodigoIntegracao;
            request.RGPARCEIRO = tomador?.CodigoIntegracao;
            request.GVAOBSERVAT = $@"{cte.Numero.ToString() ?? string.Empty}{cte.Serie.Numero.ToString() ?? string.Empty}";
            request.SAPMESSAGES = cte.ObservacoesGerais;
            request.NATOPERACAO = cte.NaturezaDaOperacao?.Descricao ?? string.Empty;
            request.SAPNRCTE = cte.Numero.ToString() ?? string.Empty;
            request.SAPSERIES = cte.Serie.Numero.ToString() ?? string.Empty;

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento && !string.IsNullOrEmpty(cte.ChaveCTESubComp))
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico ctePai = repCTe.BuscarPorChave(cte.ChaveCTESubComp);
                request.SAPDOCFAT = ctePai?.CodigoEscrituracao ?? "0";
            }
            else
            {
                request.SAPDOCFAT = "0";
            }

            request.NRPROTOCOLO = cte.Protocolo;
            request.DATAPROTOC = cte.DataAutorizacao.HasValue ? cte.DataAutorizacao.Value.ToString("yyyy-MM-dd") : "";
            request.TIMEPROTOC = cte.DataAutorizacao.HasValue ? cte.DataAutorizacao.Value.ToTimeString(true) : "";

            if (!cancelamentoCteProtocoloRequest)
                request.STATUSEFAZ = cte.SituacaoCTeSefaz.ToString("D");
            else
            {
                request.STATUSEFAZ = "2";
                request.SAPDOCFAT = cte.CodigoEscrituracao ?? "";
                request.NRORDVDA = cte.CodigoIntegracao ?? "";
            }

            request.XMLVERSIO = 1.0m;
            request.XMLVERSIOSpecified = true;
            request.KEYREGIO = cte.ChaveAcesso.Substring(0, 2);
            request.KEYEXERCONT = cte.ChaveAcesso.Substring(2, 2);
            request.KEYMESDOC = cte.ChaveAcesso.Substring(4, 2);
            request.KEYNRCNPJ = cte.ChaveAcesso.Substring(6, 14);
            request.KEYMODNF = cte.ChaveAcesso.Substring(20, 2);
            request.KEYSERIES = cte.ChaveAcesso.Substring(22, 3);
            request.KEYDOC9POS = cte.ChaveAcesso.Substring(25, 9);
            request.KEYTPEMIS = cte.ChaveAcesso.Substring(34, 1);
            request.KEYALEATO = cte.ChaveAcesso.Substring(35, 8);
            request.KEYDIGVERF = cte.ChaveAcesso.Substring(43, 1);
            request.CHAVEACESSO = cte.ChaveAcesso;
            request.USER_TMS = usario?.Nome ?? string.Empty;
            request.PEDAGIO = cte.ComponentesPrestacao.Where(o => o.NomeCTe == "PEDÁGIO").Select(x => x.Valor).FirstOrDefault();
            request.PEDAGIOSpecified = true;
            request.VLRICMSST = 0;
            request.VLRICMSSTSpecified = true;
            request.VLRICMSST2 = 0;
            request.VLRICMSST2Specified = true;

            request.ID_CONTROLE = "MULTI" + (cte.Codigo.ToString() ?? string.Empty);

            return request;
        }

        private WSSAPNSFE.VendaFrete.ZBRSDS0029 PreencherRequestVendaFrete(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Usuario usario, bool cancelamentoCteProtocoloRequest = false)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(cte.Tomador.CPF_CNPJ.ToDouble());
            Dominio.Entidades.Cliente clienteOrigem = repCliente.BuscarPorCPFCNPJ(cte.Expedidor != null ? cte.Expedidor.CPF_CNPJ.ToDouble() : cte.Remetente.CPF_CNPJ.ToDouble());
            Dominio.Entidades.Cliente clienteDestino = repCliente.BuscarPorCPFCNPJ(cte.Recebedor != null ? cte.Recebedor.CPF_CNPJ.ToDouble() : cte.Destinatario.CPF_CNPJ.ToDouble());

            WSSAPNSFE.VendaFrete.ZBRSDS0029 request = new WSSAPNSFE.VendaFrete.ZBRSDS0029();
            request.ORIGEM = "0022";
            request.TPORDVDA = "ZSE1";
            request.FLAGREFATURA = string.Empty;
            request.NRCONTRATO = tomador?.CodigoCompanhia;
            request.NRPEDIDOCLI = cte.Numero.ToString() ?? string.Empty;
            request.DTPEDIDOCLI = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("yyyy-MM-dd") : string.Empty;
            request.DATA_EMIS = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("yyyy-MM-dd") : string.Empty;
            request.CONDPAGTO = "Z000";
            request.FORMAPAGO = cte.CondicaoPagamento2;
            request.SAPSERVICO = "5100000003";
            request.CODCFOP = (cte.CFOP != null && cte.CFOP.CFOPComExtensao.EndsWith("AA")) ? cte.CFOP.CFOPComExtensao : $"{cte.CFOP.CFOPComExtensao}AA";
            request.CLUCRO = cte.CentroResultadoFaturamento?.PlanoContabilidade ?? string.Empty;
            request.SAPPOSITEM = "0";
            request.FILIALCENTRO = cte.Empresa?.CNPJ ?? string.Empty;
            request.SAPQTDE = 1;
            request.SAPQTDESpecified = true;
            request.PRECOBRUTO1 = Math.Round(cte.ValorPrestacaoServico, 2);
            request.PRECOBRUTO1Specified = true;
            request.PRECOBRUTO2 = Math.Round(cte.ValorPrestacaoServico, 2);
            request.PRECOBRUTO2Specified = true;
            SetarValoresICMsNFSe(ref request, cte);
            SetarValoresPISCOFINSNFSe(ref request, cte);
            request.AGPARCEIRO = clienteOrigem?.CodigoIntegracao;
            request.WEPARCEIRO = clienteDestino?.CodigoIntegracao;
            request.REPARCEIRO = tomador?.CodigoIntegracao;
            request.RGPARCEIRO = tomador?.CodigoIntegracao;
            request.SAPMESSAGES = cte.ObservacoesGerais;
            request.SAPNRRPS = cte.Numero.ToString() ?? string.Empty;
            request.SAPSERIES = cte.Serie.Numero.ToString() ?? string.Empty;
            request.PREFNFSE = "0";
            request.CHECOD = "";
            request.TIPONFS = "01";
            //request.PEDAGIO = 0; wsdl recebido não possui tal 
            request.USER_TMS = usario?.Nome ?? string.Empty;
            request.ID_CONTROLE = "MULTI" + (cte.Codigo.ToString() ?? string.Empty);


            return request;
        }

        private Servicos.ServicoSAP.AV.DT_Desconto_Avaria_req PreencherRequestDescontoAvaria(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato)
        {
            Servicos.ServicoSAP.AV.DT_Desconto_Avaria_req request = new ServicoSAP.AV.DT_Desconto_Avaria_req();

            Servicos.ServicoSAP.AV.DT_Desconto_Avaria_reqItem T_HEADER = new ServicoSAP.AV.DT_Desconto_Avaria_reqItem
            {
                DAT_DOC = DateTime.Now.AddDays(1),
                TIPO_DOC = "AV",
                EMPRESA = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete?.CodigoCompanhia ?? string.Empty,
                DATA_LANC = DateTime.Now,
                MOEDA = "BRL",
                REFERENCIA = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete?.NumeroContrato.ToString() ?? string.Empty,
                TEXTO_CAB = $"RECIBO: {integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete?.NumeroContrato ?? 0}",
                NUM_FILIAL = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.TransportadorTerceiro?.CodigoSap ?? string.Empty,
                DOC_CONT = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete?.TransportadorTerceiro?.CodigoDocumento ?? string.Empty,
            };

            Servicos.ServicoSAP.AV.DT_Desconto_Avaria_reqItem1 T_ITEM_AVARIA = new ServicoSAP.AV.DT_Desconto_Avaria_reqItem1
            {
                CHAVE_LANC = "50",
                CONTA_AVARIA = "4111911001",
                MONTANTE = integracaoContrato.ContratoFreteAcrescimoDesconto.Valor,
                LOCAL_NEG = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.TransportadorTerceiro?.CodigoSap ?? string.Empty,
                COND_PAG = "Z000",
                DATA_BASE = DateTime.Now,
                REF_PAG = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete?.NumeroContrato.ToString() ?? string.Empty,
                ATRIBUICAO = "0002008440",
                BANCO_EMPRESA = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete?.TransportadorTerceiro?.Banco?.CodigoIntegracao ?? string.Empty,
                ID_CONTA = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete?.TransportadorTerceiro?.NumeroConta ?? string.Empty,
                CENTRO_CUSTO = "F65137-184"
            };

            ServicoSAP.AV.DT_Desconto_Avaria_reqItem2 T_ITEM_PROP = new ServicoSAP.AV.DT_Desconto_Avaria_reqItem2
            {
                CHAVE_LANC = "31",
                MONTANTE = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete?.ValorTotalDescontoSaldo ?? 0,
                COND_PAG = "Z000",
                DATA_BASE = DateTime.Now,
                BLOQ_PAG = "C",
                REF_PAG = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete?.NumeroContrato.ToString() ?? string.Empty,
                ATRIBUICAO = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.TransportadorTerceiro?.CodigoSap ?? string.Empty,
                COD_PROP = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.TransportadorTerceiro?.CodigoSap ?? string.Empty,
                LOCAL_NEGOCIO = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.TransportadorTerceiro?.CodigoSap ?? string.Empty,
            };

            request.T_HEADER = new ServicoSAP.AV.DT_Desconto_Avaria_reqItem[] { T_HEADER };
            request.T_ITEM_AVARIA = new ServicoSAP.AV.DT_Desconto_Avaria_reqItem1[] { T_ITEM_AVARIA };
            request.T_ITEM_PROP = new ServicoSAP.AV.DT_Desconto_Avaria_reqItem2[] { T_ITEM_PROP };

            return request;
        }

        private Servicos.ServicoSAP_SU.DT_Criar_Saldo_Frete_req PreencherRequisicaoCriarSaldoFrete(Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao integracao)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(integracao.CargaRegistroEncerramento.Carga.Codigo);

            Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9 repIntegracaoSAPV9 = new Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9(_unitOfWork);
            string protocolo = repIntegracaoSAPV9.BuscarProtocoloPorCarga(integracao.CargaRegistroEncerramento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_SU);

            Servicos.ServicoSAP_SU.DT_Criar_Saldo_Frete_req request = new Servicos.ServicoSAP_SU.DT_Criar_Saldo_Frete_req();

            Servicos.ServicoSAP_SU.DT_Criar_Saldo_Frete_reqItem T_HEADER = new Servicos.ServicoSAP_SU.DT_Criar_Saldo_Frete_reqItem
            {
                DOC_CONT = protocolo,
                DATA_DOC = DateTime.Now,
                TIPO_DOC = "SU",
                EMPRESA = integracao.CargaRegistroEncerramento.Carga.Empresa?.CodigoIntegracao ?? string.Empty,
                DATA_LANC = DateTime.Now,
                MOEDA = "BRL",
                REFERENCIA = contratoFrete?.NumeroContrato.ToString() ?? string.Empty,
                TEXTO_CABECALHO = $"SALDO RECIBO {contratoFrete?.NumeroContrato ?? 0}",
                TEXTO_COMP = $"BAIXA TOTAL RECIBO {contratoFrete?.NumeroContrato ?? 0}",
                NUM_FILIAL = integracao.CargaRegistroEncerramento.Carga.Empresa?.CodigoCentroCusto ?? string.Empty,
            };

            Servicos.ServicoSAP_SU.DT_Criar_Saldo_Frete_reqItem1 T_ITEM_CARUANA = new Servicos.ServicoSAP_SU.DT_Criar_Saldo_Frete_reqItem1
            {
                CHAVE_LANC = "31",
                COD_CARTAO = "1000J027",
                MONTANTE = contratoFrete?.SaldoAReceber ?? 0,
                LOCAL_NEG = integracao.CargaRegistroEncerramento.Carga.Empresa?.CodigoCentroCusto ?? string.Empty,
                COND_PAG = "Z000",
                DATA_BASE = proximoDiaUltil(),
                BLOQ_PAG = "C",
                FORMA_PAG = "U",
                REF_PAG = contratoFrete?.NumeroContrato.ToString() ?? string.Empty,
                ATRIBUICAO = contratoFrete.TransportadorTerceiro?.CodigoIntegracao ?? string.Empty,
                BANCO_EMPRESA = contratoFrete?.TransportadorTerceiro?.Banco?.CodigoIntegracao ?? string.Empty,
                ID_CONTA = contratoFrete?.TransportadorTerceiro?.NumeroConta ?? string.Empty,
                TEXTO = $"SALDO RECIBO {contratoFrete?.NumeroContrato ?? 0}",
                COD_FORN = contratoFrete.TransportadorTerceiro?.CodigoIntegracao ?? string.Empty,
            };

            request.T_HEADER = new Servicos.ServicoSAP_SU.DT_Criar_Saldo_Frete_reqItem[] { T_HEADER };
            request.T_ITEM_CARUANA = new Servicos.ServicoSAP_SU.DT_Criar_Saldo_Frete_reqItem1[] { T_ITEM_CARUANA };

            return request;
        }

        private Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Req PreencherRequestConsultaCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Req request = new Servicos.ServicoSAP.ConsultaCTe.DT_Consulta_Doc_Req();

            request.ID_CONTROLE = "MULTI" + (cte.Codigo.ToString() ?? string.Empty);
            request.NUM_DOC_EXT = cte.Numero.ToString() ?? string.Empty;
            request.SERIE = cte.Serie?.Numero.ToString() ?? string.Empty;
            request.CENTRO = cte.Empresa?.CodigoCentroCusto ?? string.Empty;

            return request;
        }

        private Servicos.ServicoSAP.SolicitaCancelamento.DT_Solicita_Cancel_ST_req PreencherRequestSolicitacaoCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento integracao)
        {
            Servicos.ServicoSAP.SolicitaCancelamento.DT_Solicita_Cancel_ST_req request = new ServicoSAP.SolicitaCancelamento.DT_Solicita_Cancel_ST_req();

            List<Servicos.ServicoSAP.SolicitaCancelamento.DT_Solicita_Cancel_ST_reqItem> itens = new List<ServicoSAP.SolicitaCancelamento.DT_Solicita_Cancel_ST_reqItem>();

            itens.Add(new ServicoSAP.SolicitaCancelamento.DT_Solicita_Cancel_ST_reqItem()
            {
                DOC_CONT_ST = "0010000110",
                EMPRESA = "1000",
                EXERCICIO = DateTime.Now.Year.ToString(),
                FORNECEDOR = integracao.CargaCancelamento.Carga.Terceiro?.CodigoIntegracao ?? string.Empty
            });

            request.T_DOC = itens.ToArray();

            return request;
        }

        private Servicos.ServicoSAP.SolicitaCancelamentoCTe.DT_Solicita_Cancelamento_req PreencherRequestSolicitacaoCancelamentoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados integracao)
        {
            Servicos.ServicoSAP.SolicitaCancelamentoCTe.DT_Solicita_Cancelamento_req request = new Servicos.ServicoSAP.SolicitaCancelamentoCTe.DT_Solicita_Cancelamento_req();

            request.ZSOLICITASTATUS = new ServicoSAP.SolicitaCancelamentoCTe.DT_Solicita_Cancelamento_reqZSOLICITASTATUS()
            {
                ZORIGEM = "0022",
                ZSAPEMPRESA = "1000",
                ZDATASOLICTE = integracao.CargaCTe.CTe?.DataEmissao.Value.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd"),
                ZSAPDOCFAT = "0137650802",
            };

            return request;
        }

        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao integracao, InspectorBehavior inspector, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(inspector.LastRequestXML) && string.IsNullOrWhiteSpace(inspector.LastResponseXML))
                return;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

            arquivoIntegracao.ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            arquivoIntegracao.ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);
            arquivoIntegracao.Data = integracao.DataIntegracao;
            arquivoIntegracao.Mensagem = mensagem;
            arquivoIntegracao.Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (integracao.ArquivosTransacao == null)
                integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            integracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao integracao, InspectorBehavior inspector, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(inspector.LastRequestXML) && string.IsNullOrWhiteSpace(inspector.LastResponseXML))
                return;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

            arquivoIntegracao.ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            arquivoIntegracao.ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);
            arquivoIntegracao.Data = integracao.DataIntegracao;
            arquivoIntegracao.Mensagem = mensagem;
            arquivoIntegracao.Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (integracao.ArquivosTransacao == null)
                integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            integracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao, InspectorBehavior inspector, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(inspector.LastRequestXML) && string.IsNullOrWhiteSpace(inspector.LastResponseXML))
                return;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();

            arquivoIntegracao.ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            arquivoIntegracao.ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);
            arquivoIntegracao.Data = integracao.DataIntegracao;
            arquivoIntegracao.Mensagem = mensagem;
            arquivoIntegracao.Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (integracao.ArquivosTransacao == null)
                integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>();

            integracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private ServicoSAP.AgruparFatura.SI_Agrupar_Fatura_ASync_OUTClient ObterClienteIntegracaoFatura(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP configuracao)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configuracao.URLIntegracaoFatura);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (configuracao.URLIntegracaoFatura.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            ServicoSAP.AgruparFatura.SI_Agrupar_Fatura_ASync_OUTClient client = new ServicoSAP.AgruparFatura.SI_Agrupar_Fatura_ASync_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = configuracao.Usuario;
            client.ClientCredentials.UserName.Password = configuracao.Senha;

            return client;
        }

        private Servicos.ServicoSAP.EstornaFatura.SI_Estorno_Agrup_AsyncClient ObterClienteIntegracaoEstornoFatura(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP configuracao)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configuracao.URLIntegracaoEstornoFatura);

            if ("http://sapwsintdev.simpar.com.br/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_TMS&receiverParty=&receiverService=&interface=SI_Estorno_Agrup_Async&interfaceNamespace=http://jsl.com.br/Estorno_Agrup_Async_FI" != configuracao.URLIntegracaoEstornoFatura)
            {

            }

            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            binding.OpenTimeout = TimeSpan.FromSeconds(30);
            binding.CloseTimeout = TimeSpan.FromSeconds(30);

            if (configuracao.URLIntegracaoFatura.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            Servicos.ServicoSAP.EstornaFatura.SI_Estorno_Agrup_AsyncClient client = new Servicos.ServicoSAP.EstornaFatura.SI_Estorno_Agrup_AsyncClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = configuracao.Usuario;
            client.ClientCredentials.UserName.Password = configuracao.Senha;

            return client;
        }

        private ServicoSAP.AgruparFatura.DT_Agrupar_Fatura_reqI_DOC[] PreencherRequisicaoIntegracaoFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes)
        {
            List<ServicoSAP.AgruparFatura.DT_Agrupar_Fatura_reqI_DOC> request = new List<ServicoSAP.AgruparFatura.DT_Agrupar_Fatura_reqI_DOC>();
            Dominio.Entidades.Embarcador.Fatura.FaturaParcela parcela = fatura.Parcelas.FirstOrDefault();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                request.Add(new ServicoSAP.AgruparFatura.DT_Agrupar_Fatura_reqI_DOC
                {
                    ID_CONTROLE = "MULTI" + (fatura.Codigo.ToString() ?? string.Empty),
                    EMPRESA = "1000",
                    ITEM_DOC = cte.Numero.ToString() ?? string.Empty,
                    CLIENTE = fatura.ClienteTomadorFatura?.CodigoIntegracao ?? string.Empty,
                    ORIGEM = "0022",
                    DOCUMENTO = fatura.Numero.ToString() ?? string.Empty,
                    NUM_FATURA = cte.CodigoEscrituracao?.ToString() ?? string.Empty,
                    DT_VENC_AGRUP = parcela != null ? parcela.DataVencimento : DateTime.MinValue,
                    DIVISAO = cte.Empresa?.CodigoIntegracao ?? string.Empty,
                    FILIAL = cte.Empresa?.CodigoCentroCusto ?? string.Empty,
                    CENTRO_LUCRO = cte.CentroResultadoFaturamento?.PlanoContabilidade ?? string.Empty,
                    CANAL_DISTRIBUICAO = "10",
                    SETOR_ATIVIDADE = "00",
                    DATA_DOC = cte.DataEmissao.HasValue ? cte.DataEmissao.Value : DateTime.MinValue
                });
            }
            return request.ToArray();
        }

        private Servicos.ServicoSAP.EstornaFatura.DT_Estorno_Agrup_Async PreencherRequisicaoIntegracaoEstornoFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes)
        {
            Servicos.ServicoSAP.EstornaFatura.DT_Estorno_Agrup_Async Retorno = new Servicos.ServicoSAP.EstornaFatura.DT_Estorno_Agrup_Async();

            List<Servicos.ServicoSAP.EstornaFatura.DT_Estorno_Agrup_AsyncItem> iDoc = new List<Servicos.ServicoSAP.EstornaFatura.DT_Estorno_Agrup_AsyncItem>();
            Dominio.Entidades.Embarcador.Fatura.FaturaParcela parcela = fatura.Parcelas.FirstOrDefault();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                iDoc.Add(new Servicos.ServicoSAP.EstornaFatura.DT_Estorno_Agrup_AsyncItem
                {
                    ID_CONTROLE = "MULTI" + (fatura.Codigo.ToString() ?? string.Empty),
                    EMPRESA = "1000",
                    ITEM_DOC = cte.Numero.ToString() ?? string.Empty,
                    CLIENTE = fatura.ClienteTomadorFatura?.CodigoIntegracao ?? string.Empty,
                    ORIGEM = "0022",
                    DOC_AGRUPAMENTO = fatura.Numero.ToString() ?? string.Empty, //cte.CodigoEscrituracao.ToString() ?? string.Empty
                });
            }
            Retorno.I_DOC = iDoc.ToArray();
            return Retorno;
        }

        private void AtualizarDadosCTE(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string docFatura, string docVenda)
        {
            if (cte == null || (string.IsNullOrWhiteSpace(docFatura) && string.IsNullOrWhiteSpace(docVenda)))
                return;

            Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            cte.CodigoEscrituracao = docFatura;
            cte.CodigoIntegracao = docVenda;
            repCTE.Atualizar(cte);
        }

        private void SetarValoresICMs(ref Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024 request, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {

            decimal baseICMS = cte.CST == "00" || cte.CST == "90" || cte.CST == "51" ? cte.BaseCalculoICMS : 0;
            decimal isencaoICMS = cte.CST == "40" ? cte.BaseCalculoICMS : cte.BaseCalculoICMS == 0 ? Math.Round(cte.ValorPrestacaoServico, 2) : 0;
            decimal outraBaseICMS = cte.CST == "60" ? cte.BaseCalculoICMS : 0;
            decimal aliquotaICMS = cte.CST == "00" || cte.CST == "90" || cte.CST == "60" ? cte.AliquotaICMS : 0;
            decimal valorICMS = cte.CST == "00" || cte.CST == "90" ? cte.ValorICMS : 0;


            request.BASEICMS = Math.Round(baseICMS, 2);
            request.ISENCAOICMS = Math.Round(isencaoICMS, 2);
            request.OTRBASEICMS = Math.Round(outraBaseICMS, 2);
            request.ALIQICMS = Math.Round(aliquotaICMS, 2);
            request.VLRICMS = Math.Round(valorICMS, 2);
            request.CST_ICMS = cte.CST;

            request.BASEICMSSpecified = true;
            request.ISENCAOICMSSpecified = true;
            request.OTRBASEICMSSpecified = true;
            request.ALIQICMSSpecified = true;
            request.VLRICMSSpecified = true;

        }

        private void SetarValoresICMsNFSe(ref WSSAPNSFE.VendaFrete.ZBRSDS0029 request, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            decimal baseICMS = cte.CST == "00" || cte.CST == "90" || cte.CST == "51" ? cte.BaseCalculoICMS : 0;
            decimal isencaoICMS = cte.CST == "40" ? cte.BaseCalculoICMS : cte.BaseCalculoICMS == 0 ? Math.Round(cte.ValorPrestacaoServico, 2) : 0;
            decimal outraBaseICMS = cte.CST == "60" ? cte.BaseCalculoICMS : 0;
            decimal aliquotaICMS = cte.CST == "00" || cte.CST == "90" || cte.CST == "60" ? cte.AliquotaICMS : 0;
            decimal valorICMS = cte.CST == "00" || cte.CST == "90" ? cte.ValorICMS : 0;
            decimal valorICMSST = cte.CST == "60" ? cte.ValorICMS : 0;

            request.BASEICMS = Math.Round(baseICMS, 2);
            request.ISENCAOICMS = Math.Round(isencaoICMS, 2);
            request.OTRBASEICMS = Math.Round(outraBaseICMS, 2);
            request.ALIQICMS = Math.Round(aliquotaICMS, 2);
            request.VLRICMS = Math.Round(valorICMS, 2);
            request.CST_ICMS = cte.CST;

            request.BASEICMSSpecified = true;
            request.ISENCAOICMSSpecified = true;
            request.OTRBASEICMSSpecified = true;
            request.ALIQICMSSpecified = true;
            request.VLRICMSSpecified = true;
        }

        private void SetarValoresPISCOFINS(ref Servicos.ServicoSAP.EnviaVendaFrete.ZBRSDS0024 request, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.Entidades.ConfiguracaoEmpresa configEmpresa = cte.Empresa?.Configuracao;
            decimal baseCalculo = Math.Round(cte.ValorPrestacaoServico, 2);
            decimal aliquotaCOFINS = configEmpresa?.AliquotaCOFINS ?? 0m;
            decimal aliquotaPIS = configEmpresa?.AliquotaPIS ?? 0m;

            request.BASECOFINS = Math.Round(baseCalculo, 2);
            request.BASEXCLCOFINS = 0;
            request.ALIQCOFINS = Math.Round(aliquotaCOFINS, 2);
            request.VLRCOFINS = Math.Round(baseCalculo * (aliquotaCOFINS / 100), 2, MidpointRounding.AwayFromZero);
            request.CST_COFINS = "01";

            request.BASEPIS = Math.Round(baseCalculo, 2);
            request.BASEXCLPIS = 0;
            request.ALIQPIS = Math.Round(aliquotaPIS, 2);
            request.VLRPIS = Math.Round(baseCalculo * (aliquotaPIS / 100), 2, MidpointRounding.AwayFromZero);
            request.CST_PIS = "01";

            request.BASEPISSpecified = true;
            request.BASEXCLPISSpecified = true;
            request.ALIQPISSpecified = true;
            request.VLRPISSpecified = true;
            request.BASECOFINSSpecified = true;
            request.BASEXCLCOFINSSpecified = true;
            request.ALIQCOFINSSpecified = true;
            request.VLRCOFINSSpecified = true;
        }

        private void SetarValoresPISCOFINSNFSe(ref WSSAPNSFE.VendaFrete.ZBRSDS0029 request, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.Entidades.ConfiguracaoEmpresa configEmpresa = cte.Empresa?.Configuracao;
            decimal baseCalculo = Math.Round(cte.ValorPrestacaoServico, 2);
            decimal aliquotaCOFINS = configEmpresa?.AliquotaCOFINS ?? 0m;
            decimal aliquotaPIS = configEmpresa?.AliquotaPIS ?? 0m;

            request.BASECOFINS = Math.Round(baseCalculo, 2);
            request.BASEXCLCOFINS = 0;
            request.ALIQCOFINS = Math.Round(aliquotaCOFINS, 2);
            request.VLRCOFINS = Math.Round(baseCalculo * (aliquotaCOFINS / 100), 2, MidpointRounding.AwayFromZero);
            request.CST_COFINS = "01";

            request.BASEPIS = Math.Round(baseCalculo, 2);
            request.BASEXCLPIS = 0;
            request.ALIQPIS = Math.Round(aliquotaPIS, 2);
            request.VLRPIS = Math.Round(baseCalculo * (aliquotaPIS / 100), 2, MidpointRounding.AwayFromZero);
            request.CST_PIS = "01";

            request.ALIQISSPREST = 0;
            request.VLRISSPREST = 0;
            request.ALIQISSTOMA = cte.AliquotaCSLL; //aliquota da carga


            request.BASEPISSpecified = true;
            request.BASEXCLPISSpecified = true;
            request.ALIQPISSpecified = true;
            request.VLRPISSpecified = true;
            request.BASECOFINSSpecified = true;
            request.BASEXCLCOFINSSpecified = true;
            request.ALIQCOFINSSpecified = true;
            request.VLRCOFINSSpecified = true;
            request.ALIQISSPRESTSpecified = true;
            request.VLRISSPRESTSpecified = true;
            request.ALIQISSTOMASpecified = true;
        }

        private DateTime proximoDiaUltil()
        {
            DateTime data = DateTime.Now;
            Servicos.Embarcador.Configuracoes.Feriado servicoFeriado = new Configuracoes.Feriado(_unitOfWork);
            List<DateTime> datasComFeriado = servicoFeriado.ObterDatasComFeriado(data, data.AddDays(60));
            do
            {
                data = data.AddDays(1);
            } while (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday || datasComFeriado.Contains(data.Date));
            return data;
        }

        private string IntegrarDadosCargaAPI4<T>(T integracao, Dominio.Entidades.Embarcador.Cargas.Carga carga, out string jsonRequisicao, out string jsonRetorno, string operacao, string protocolo) where T : Dominio.Entidades.Embarcador.Integracao.Integracao
        {
            jsonRequisicao = string.Empty;
            jsonRetorno = string.Empty;
            string numeroRetornoSAP = string.Empty;

            try
            {
                integracao.NumeroTentativas++;
                integracao.DataIntegracao = DateTime.Now;

                ObterConfiguracaoIntegracaoSAP_API4();

                IntegracaoEnvioDadosCarga dados = MontarDadosCarga(carga, operacao, protocolo);
                jsonRequisicao = JsonConvert.SerializeObject(dados, Newtonsoft.Json.Formatting.Indented);

                System.Net.Http.HttpClient client = ObterClient(_configuracaoIntegracaoSAP_API4.UsuarioSAP_API4, _configuracaoIntegracaoSAP_API4.SenhaSAP_API4);
                System.Net.Http.StringContent content = new System.Net.Http.StringContent(jsonRequisicao, System.Text.Encoding.UTF8, "application/json");

                System.Net.Http.HttpResponseMessage result = client.PostAsync(_configuracaoIntegracaoSAP_API4.URLSAP_API4, content).Result;
                jsonRetorno = result.Content.ReadAsStringAsync().Result;
                RetornoIntegracaoDadosCarga response = JsonConvert.DeserializeObject<RetornoIntegracaoDadosCarga>(jsonRetorno);

                if (string.IsNullOrWhiteSpace(response?.NumeroDocumentoSAP))
                    throw new ServicoException("Tentativa de integração SAP API4 falhou: " + response.Texto);

                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracao.ProblemaIntegracao = "Integrado com Sucesso.";
                numeroRetornoSAP = response.NumeroDocumentoSAP;
            }
            catch (BaseException ex)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "LogIntegracaoSAP_API4");

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = $"Erro ao integrar com SAP API4";
            }

            return numeroRetornoSAP;
        }

        private IntegracaoEnvioDadosCarga MontarDadosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, string tipoIntegracao, string protocolo)
        {
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Usuario motorista = repCargaMotorista.BuscarPrimeiroMotoristaPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            return new IntegracaoEnvioDadosCarga
            {
                RotaMulti = protocolo,
                ProtocoloCarga = carga.Protocolo.ToString(),
                Processo = tipoIntegracao,
                Transportadora = carga.Empresa?.CodigoIntegracao ?? string.Empty,
                Veiculo = new Dominio.ObjetosDeValor.Embarcador.Integracao.SAP.Veiculo
                {
                    Tipo = carga.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty,
                    Placa = carga.Veiculo?.Placa ?? string.Empty
                },
                Motorista = new Motorista
                {
                    Nome = motorista?.Nome ?? string.Empty,
                    DocumentoIdentidade = motorista?.RG ?? string.Empty,
                },
                ValorFrete = carga.ValorFrete.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
                Quilometragem = carga.DadosSumarizados?.Distancia.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
                TotalEntregas = cargasPedido.Count.ToString(),
                Itens = ObterPedidosCarga(cargasPedido)
            };
        }

        private List<ItemEntrega> ObterPedidosCarga(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido)
        {
            if (cargasPedido == null)
                return new List<ItemEntrega> { };

            return cargasPedido.Select(cargaPedido => new ItemEntrega
            {
                SequenciaEntrega = cargaPedido.OrdemEntrega.ToString() ?? string.Empty,
                Entrega = cargaPedido.Pedido.NumeroPedidoEmbarcador ?? string.Empty,
                DataEsperada = cargaPedido.Pedido.PrevisaoEntrega?.ToDateTimeString(showSeconds: true) ?? string.Empty,
                DataEstimada = cargaPedido.Pedido.DataPrevisaoSaida?.ToDateTimeString(showSeconds: true) ?? string.Empty,
                DataCarregamento = cargaPedido.Carga.DataCarregamentoCarga?.ToDateTimeString(showSeconds: true) ?? string.Empty,
                Latitude = cargaPedido.Pedido.Destino?.Latitude.ToString() ?? string.Empty,
                Longitude = cargaPedido.Pedido.Destino?.Longitude.ToString() ?? string.Empty
            }).ToList();
        }

        private static System.Net.Http.HttpClient ObterClient(string usuario, string senha)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            client.Timeout = TimeSpan.FromMinutes(3);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{usuario}:{senha}")));

            return client;
        }

        private void ObterConfiguracaoIntegracaoSAP_API4()
        {
            if (_configuracaoIntegracaoSAP_API4 != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSapAPI4 repIntegracaoSap_API4 = new Repositorio.Embarcador.Configuracoes.IntegracaoSapAPI4(_unitOfWork);

            _configuracaoIntegracaoSAP_API4 = repIntegracaoSap_API4.BuscarPrimeiroRegistro();

            if (_configuracaoIntegracaoSAP_API4 == null || !_configuracaoIntegracaoSAP_API4.PossuiIntegracaoSAP_API4)
                throw new ServicoException("Não existe configuração de integração disponível para SAP API4.");
        }

        #endregion
    }
}
