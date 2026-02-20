using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.Integracao.Nstech
{
    public class IntegracaoMotorista : IntegracaoNSTech
    {
        #region Propriedades Privadas

        private static string _caminhoArquivosIntegracao;
        private static AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private static Repositorio.UnitOfWork _unitOfWork;

        private static string _token;

        #endregion

        #region Construtores

        public IntegracaoMotorista(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            _caminhoArquivosIntegracao = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
        }


        #endregion


        #region Métodos Globais

        public void IntegrarMotorista(ref Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracaoMotorista)
        {
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoNSTech repIntegracaoNstech = new Repositorio.Embarcador.Configuracoes.IntegracaoNSTech(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech configuracaoIntegracao = repIntegracaoNstech.Buscar();

            if (ValidarConfiguracaoIntegracaoNSTech(integracaoMotorista, configuracaoIntegracao))
            {
                try
                {
                    InspectorBehavior inspector = new InspectorBehavior();
                    int sequenciaIntegracao = repMotoristaIntegracao.ContarPorMotoristaETipo(integracaoMotorista.Motorista.Codigo, integracaoMotorista.TipoIntegracao.Tipo);
                    _token = base.ObterTokenIntegracaoNSTech(configuracaoIntegracao);

                    integracaoMotorista.DataIntegracao = DateTime.Now;

                    string jsonRequest = string.Empty;
                    string response = string.Empty;
                    HttpResponseMessage jsonresult;

                    if (CadastrarMotorista(integracaoMotorista, sequenciaIntegracao))
                        //cadastro
                        response = requestCadastroMotorista(integracaoMotorista, configuracaoIntegracao, out jsonRequest, out jsonresult);
                    else
                        //consulta
                        response = requestConsultaMotorista(integracaoMotorista, configuracaoIntegracao, out jsonRequest, out jsonresult);

                    string msgErro = "";
                    if (jsonresult.IsSuccessStatusCode)
                    {
                        dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(response);

                        if (string.IsNullOrEmpty((string)objetoRetorno?.body?.Erro) && objetoRetorno?.body?.retorno == null && string.IsNullOrEmpty((string)objetoRetorno?.msg) && string.IsNullOrEmpty((string)objetoRetorno.data[0]?.Erro) && string.IsNullOrEmpty((string)objetoRetorno.data[0]?.buonny[0]?.Fault?.detail?.Exception?.erro))
                        {
                            integracaoMotorista.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                            integracaoMotorista.ProblemaIntegracao = "Sucesso - Protocolo " + (string)objetoRetorno?.body.MonitoramentoId + ".";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty((string)objetoRetorno?.body?.Mensagem))
                                msgErro = (string)objetoRetorno.body.Mensagem;
                            else if (!string.IsNullOrEmpty((string)objetoRetorno?.msg))
                                msgErro = (string)objetoRetorno.msg;
                            else if (objetoRetorno?.body?.retorno != null && !string.IsNullOrEmpty((string)objetoRetorno?.body?.retorno[0]?.Aviso))
                                msgErro = (string)objetoRetorno.body.retorno[0].Aviso;
                            else if (!string.IsNullOrEmpty((string)objetoRetorno?.body?.Erro))
                                msgErro = (string)objetoRetorno?.body?.Erro;
                            else if (!string.IsNullOrEmpty((string)objetoRetorno.data[0]?.Erro))
                                msgErro = (string)objetoRetorno.data[0]?.Erro;
                            else if (!string.IsNullOrEmpty((string)objetoRetorno.data[0]?.buonny[0]?.Fault?.detail?.Exception?.erro))
                                msgErro = (string)objetoRetorno.data[0]?.buonny[0]?.Fault?.detail?.Exception?.erro;

                            integracaoMotorista.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            integracaoMotorista.ProblemaIntegracao = msgErro;
                        }
                    }
                    else
                    {
                        msgErro = "Não foi possível obter o retorno: StatusCode " + jsonresult.StatusCode.ToString();

                        integracaoMotorista.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        integracaoMotorista.ProblemaIntegracao = "Não foi possível obter o retorno: StatusCode " + jsonresult.StatusCode.ToString();
                        repMotoristaIntegracao.Atualizar(integracaoMotorista);
                    }

                    AdicionarArquivosIntegracao(ref integracaoMotorista, response, jsonRequest, msgErro);
                    repMotoristaIntegracao.Atualizar(integracaoMotorista);

                }
                catch (ServicoException ex)
                {
                    integracaoMotorista.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoMotorista.ProblemaIntegracao = ex.Message;
                    repMotoristaIntegracao.Atualizar(integracaoMotorista);

                }
                catch (Exception ex)
                {
                    integracaoMotorista.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoMotorista.ProblemaIntegracao = ex.Message;
                    repMotoristaIntegracao.Atualizar(integracaoMotorista);
                }
            }
        }

        #endregion

        #region Métodos Privados

        private string requestCadastroMotorista(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracaoMotorista, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech configuracaoIntegracao, out string jsonRequest, out HttpResponseMessage jsonresult)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoSolicitacaoAnaliseCadastral ObjetorequisicaoMotorista = ObterObjetoCadastroMotorista(integracaoMotorista);

            ObjetorequisicaoMotorista.solicitante_id = configuracaoIntegracao.IDAutenticacao;
            ObjetorequisicaoMotorista.solicitante_token = _token;

            jsonRequest = JsonConvert.SerializeObject(ObjetorequisicaoMotorista, Formatting.Indented);
            HttpClient client = ObterClient(configuracaoIntegracao.UrlIntegracaoSolicitacaoCadastral);

            var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
            jsonresult = client.PostAsync(configuracaoIntegracao.UrlIntegracaoSolicitacaoCadastral, content).Result;

            if ((int)jsonresult.StatusCode == 308)
            {
                string redirectUrl = jsonresult.Headers.Location.ToString();
                content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpClient clientRedirect = ObterClient(redirectUrl);
                jsonresult = clientRedirect.PostAsync(redirectUrl, content).Result;
            }

            return jsonresult.Content.ReadAsStringAsync().Result;
        }

        private string requestConsultaMotorista(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracaoMotorista, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech configuracaoIntegracao, out string jsonRequest, out HttpResponseMessage jsonresult)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoConsultaAnaliseCadastro ObjetorequisicaoMotorista = ObterObjetoConsultaMotorista(integracaoMotorista);

            ObjetorequisicaoMotorista.solicitante_id = configuracaoIntegracao.IDAutenticacao;
            ObjetorequisicaoMotorista.solicitante_token = _token;

            jsonRequest = JsonConvert.SerializeObject(ObjetorequisicaoMotorista, Formatting.Indented);
            HttpClient client = ObterClient(configuracaoIntegracao.UrlIntegracaoVerificacaoCadastral);

            var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
            jsonresult = client.PostAsync(configuracaoIntegracao.UrlIntegracaoVerificacaoCadastral, content).Result;

            if ((int)jsonresult.StatusCode == 308)
            {
                string redirectUrl = jsonresult.Headers.Location.ToString();
                content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpClient clientRedirect = ObterClient(redirectUrl);
                jsonresult = clientRedirect.PostAsync(redirectUrl, content).Result;
            }

            return jsonresult.Content.ReadAsStringAsync().Result;
        }

        private bool ValidarConfiguracaoIntegracaoNSTech(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao motoristaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech configuracaoIntegracao)
        {
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorio = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(_unitOfWork);

            if (configuracaoIntegracao == null || string.IsNullOrEmpty(configuracaoIntegracao?.UrlAutenticacao))
            {
                motoristaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                motoristaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível com a NSTech.";

                return false;
            }

            return true;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoSolicitacaoAnaliseCadastral ObterObjetoCadastroMotorista(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracaoMotorista)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoSolicitacaoAnaliseCadastral requisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoSolicitacaoAnaliseCadastral();

            //Para cada tipo integracao completar o objeto correspondente seguindo manual de/para da NSTech (https://plataformanstech.notion.site/SCAD-Solicita-o-de-An-lise-Cadastral-Em-Atualiza-es-bbf84c50a4b04e4b9a7d5f9267cd58f0);

            switch (integracaoMotorista.TipoIntegracao.Tipo)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny:
                    requisicao = Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.RetornarObjetoRequisicaoCadastroMotorista(integracaoMotorista.Motorista, _unitOfWork);
                    break;
                default:
                    break;
            }

            return requisicao;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoConsultaAnaliseCadastro ObterObjetoConsultaMotorista(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracaoMotorista)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoConsultaAnaliseCadastro requisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoConsultaAnaliseCadastro();

            //Para cada tipo integracao completar o objeto correspondente seguindo manual de/para da NSTech (https://plataformanstech.notion.site/SCAD-Solicita-o-de-An-lise-Cadastral-Em-Atualiza-es-bbf84c50a4b04e4b9a7d5f9267cd58f0);

            switch (integracaoMotorista.TipoIntegracao.Tipo)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny:
                    requisicao = Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.RetornarObjetoConsultaCadastroMotorista(integracaoMotorista.Motorista, _unitOfWork);
                    break;
                default:
                    break;
            }

            return requisicao;
        }

        private void AdicionarArquivosIntegracao(ref Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao, string jsonRequest, string jsonResponse, string mensagem)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork, _caminhoArquivosIntegracao),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork, _caminhoArquivosIntegracao),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (integracao.ArquivosTransacao == null)
                integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            integracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private bool CadastrarMotorista(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao, int sequenciaIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            //implementar aqui para cada tipo de integracao onde pode ter o cadastro antes da consulta; 
            switch (integracao.TipoIntegracao.Tipo)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny:
                    if (configuracaoIntegracao.CadastrarMotoristaAntesConsultarBuonny && sequenciaIntegracao <= 1)
                        return true;
                    else
                        return false;
                default:
                    return false;

            }

        }

        #endregion
    }
}

