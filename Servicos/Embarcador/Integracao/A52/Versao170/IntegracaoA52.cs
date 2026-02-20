using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.A52.V170
{
    public partial class IntegracaoA52
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 _configuracaoIntegracao;
        string tokenAutenticacao = null;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoA52(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            _configuracaoIntegracao = configuracaoIntegracao;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool ObterToken(out string mensagemErro)
        {
            mensagemErro = null;

            try
            {
                if (_configuracaoIntegracao == null)
                    throw new ServicoException("Processo Abortado! Integração não configurada.");

                if (string.IsNullOrEmpty(this._configuracaoIntegracao.CPFCNPJ) || string.IsNullOrEmpty(this._configuracaoIntegracao.Senha))
                {
                    mensagemErro = "Usuário e senha não definidos.";
                    this.tokenAutenticacao = "";
                    return false;
                }

                if (!string.IsNullOrEmpty(this.tokenAutenticacao))
                    return true;

                //Montar envio
                envLogin envioWS = new envLogin();
                envioWS.username = this._configuracaoIntegracao.CPFCNPJ;
                envioWS.password = this._configuracaoIntegracao.Senha;

                //Transmite o arquivo
                var retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "login", null, true);

                //Processar Retorno
                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    mensagemErro = string.Concat("Ocorreu erro ao consumir o webservice: ", retornoWS.mensagem);

                    this.tokenAutenticacao = null;
                    return false;
                }
                else
                {
                    retLogin retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retLogin>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar token de autenticação A52: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu um erro ao obter o token; RetornoWS {0}.", retornoWS.jsonRetorno);

                        this.tokenAutenticacao = null;
                        return false;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(retorno.access_token))
                        {
                            this.tokenAutenticacao = retorno.access_token;
                            return true;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(retorno.message))
                                mensagemErro = string.Format("Error: {0}; Message: {1}.", retorno.statusCode == null ? "" : retorno.statusCode.ToString(), retorno.message);
                            else
                                mensagemErro = "Error: Ocorreu um erro ao obter o token.";

                            this.tokenAutenticacao = null;
                            return false;
                        }
                    }
                }
            }
            catch (ServicoException ex)
            {
                mensagemErro = ex.Message;

                this.tokenAutenticacao = null;
                return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao obter o tokem da Repom Frete.";

                this.tokenAutenticacao = null;
                return false;
            }
        }

        private retornoWebService TransmitirRepom(enumTipoWS tipoWS, object objEnvio, string metodo, string token, bool formUrlEncoded = false)
        {
            return this.Transmitir(tipoWS, null, objEnvio, metodo, token, formUrlEncoded);
        }

        private retornoWebService TransmitirRepom(enumTipoWS tipoWS, string parametroGET, string metodo, string token)
        {
            return this.Transmitir(tipoWS, parametroGET, null, metodo, token, true);
        }

        private retornoWebService Transmitir(enumTipoWS tipoWS, string parametroGET, object objEnvio, string metodo, string token, bool formUrlEncoded = false)
        {
            var retornoWS = new retornoWebService();

            try
            {
                if (_configuracaoIntegracao== null)
                    throw new ServicoException("Processo Abortado! Integração não configurada.");

                if (string.IsNullOrEmpty(_configuracaoIntegracao.URL))
                    throw new ServicoException("Processo Abortado! URL não definida.");

                string url = null;
                if (_configuracaoIntegracao.URL.EndsWith("/"))
                    url = _configuracaoIntegracao.URL;
                else
                    url = _configuracaoIntegracao.URL + "/";
                url += metodo;

                HttpContent content = null;
                HttpRequestMessage request = null;

                if (tipoWS == enumTipoWS.POST || tipoWS == enumTipoWS.PUT)
                {
                    retornoWS.jsonEnvio = JsonConvert.SerializeObject(objEnvio);

                    if (formUrlEncoded)
                    {
                        var values = retornoWS.jsonEnvio.FromJson<Dictionary<string, object>>();
                        Dictionary<string, string> dString = values.ToDictionary(k => k.Key, k => k.Value.ToString());
                        content = new FormUrlEncodedContent(dString);
                    }
                    else
                    {
                        content = new StringContent(JsonConvert.SerializeObject(objEnvio), Encoding.UTF8, "application/json");
                    }
                }
                else if (tipoWS == enumTipoWS.GET)
                {
                    if (!string.IsNullOrEmpty(parametroGET))
                    {
                        if (!url.EndsWith("/"))
                            url += "/";
                        url += parametroGET;
                    }

                    retornoWS.jsonEnvio = url;
                }
                else if (tipoWS == enumTipoWS.PATCH)
                {
                    if (!url.EndsWith("/"))
                        url += "/";
                    url += parametroGET;

                    request = new HttpRequestMessage(new HttpMethod("PATCH"), url);

                    if (objEnvio != null)
                    {
                        retornoWS.jsonEnvio = JsonConvert.SerializeObject(objEnvio);
                        request.Content = new StringContent(retornoWS.jsonEnvio, Encoding.UTF8, "application/json");
                    }
                    else
                    {
                        retornoWS.jsonEnvio = url;
                    }
                }

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoA52));

                // TIMEOUT EM MILISEGUNDOS
                client.Timeout = TimeSpan.FromMilliseconds(300000);

                // TLS 1.2
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = null;
                if (tipoWS == enumTipoWS.POST)
                    response = client.PostAsync(url, content).Result;
                else if (tipoWS == enumTipoWS.GET)
                    response = client.GetAsync(url).Result;
                else if (tipoWS == enumTipoWS.PATCH)
                    response = client.SendAsync(request).Result;
                else if (tipoWS == enumTipoWS.PUT)
                    response = client.PutAsync(url, content).Result;

                retornoWS.jsonRetorno = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    retornoWS.erro = true;
                    if (string.IsNullOrEmpty(retornoWS.jsonRetorno))
                        retornoWS.mensagem = string.Format(@"Não foi possível consumir o Web Service {0}: {1}.", metodo, response.RequestMessage);
                    else
                        retornoWS.mensagem = string.Format(@"Não foi possível consumir o Web Service {0}: {1};{2}RetornoWS: {3};", metodo, response.RequestMessage, System.Environment.NewLine, retornoWS.jsonRetorno);
                }
                else
                {
                    retornoWS.erro = false;
                }

            }
            catch (ServicoException ex)
            {
                retornoWS.erro = true;
                retornoWS.mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                retornoWS.erro = true;
                retornoWS.mensagem = $"Ocorreu uma falha ao consumir o metodo {metodo}.";
            }

            return retornoWS;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Configurações

        private bool PossuiIntegracaoA52(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            return !(configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.CPFCNPJ) || string.IsNullOrWhiteSpace(configuracaoIntegracao.Senha) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URL));
        }

        #endregion Métodos Privados - Configurações
    }
}
