using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utilidades.Extensions;

namespace Servicos.Embarcador.CIOT.TruckPad
{
    public partial class IntegracaoTruckPad
    {
        Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad _configuracaoIntegracaoTruckPad = null;
        string tokenAutenticacao = null;

        #region Métodos Globais

        #endregion

        #region Métodos Privados

        public Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad.retornoWebService TransmitirTruckPad(enumTipoWS tipoWS, object objEnvio, string metodo, string token, bool formUrlEncoded = false, string urlWebService = null)
        {
            return this.Transmitir(tipoWS, null, objEnvio, metodo, token, formUrlEncoded, urlWebService);
        }

        public Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad.retornoWebService TransmitirTruckPad(enumTipoWS tipoWS, string parametroGET, string metodo, string token)
        {
            return this.Transmitir(tipoWS, parametroGET, null, metodo, token, true);
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad.retornoWebService Transmitir(enumTipoWS tipoWS, string parametroGET, object objEnvio, string metodo, string token, bool formUrlEncoded = false, string urlWebService = null)
        {
            var retornoWS = new Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad.retornoWebService();

            try
            {
                if (_configuracaoIntegracaoTruckPad == null)
                    throw new ServicoException("Processo Abortado! Integração não configurada.");

                string url = null;

                if (!string.IsNullOrEmpty(urlWebService))
                    url = urlWebService;
                else
                    url = _configuracaoIntegracaoTruckPad.URLTruckPad;

                if (string.IsNullOrEmpty(url))
                    throw new ServicoException("Processo Abortado! URL não definida.");

                if (!string.IsNullOrEmpty(metodo))
                {
                    if (_configuracaoIntegracaoTruckPad.URLTruckPad.EndsWith("/"))
                        url = _configuracaoIntegracaoTruckPad.URLTruckPad;
                    else
                        url = _configuracaoIntegracaoTruckPad.URLTruckPad + "/";
                    url += metodo;
                }

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
                else if (tipoWS == enumTipoWS.DELETE)
                {
                    if (!url.EndsWith("/"))
                        url += "/";
                    url += parametroGET;

                    request = new HttpRequestMessage(new HttpMethod("DELETE"), url);

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

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTruckPad));

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
                else if (tipoWS == enumTipoWS.DELETE)
                    response = client.SendAsync(request).Result;

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

        private Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad ObterConfiguracaoTruckPad(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.CIOT.CIOTTruckPad repCIOTRepom = new Repositorio.Embarcador.CIOT.CIOTTruckPad(unidadeTrabalho);

            Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad configuracao = null;
            if (configuracaoCIOT == null)
                configuracao = repCIOTRepom.BuscarPrimeiroRegistro();
            else
                configuracao = repCIOTRepom.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

            return configuracao;
        }

        private bool ObterToken(out string mensagemErro)
        {
            mensagemErro = null;

            try
            {
                if (_configuracaoIntegracaoTruckPad == null)
                    throw new ServicoException("Processo Abortado! Integração não configurada.");

                //Caso a senha e usuario não estiverem preenchidas a autenticação será através do certificado digital;
                if (string.IsNullOrEmpty(this._configuracaoIntegracaoTruckPad.SenhaTruckPad) || string.IsNullOrEmpty(this._configuracaoIntegracaoTruckPad.UsuarioTruckPad))
                {
                    mensagemErro = "Usuário e senha não definidos.";

                    this.tokenAutenticacao = "";
                    return false;
                }

                if (!string.IsNullOrEmpty(this.tokenAutenticacao))
                {
                    return true;
                }

                //Montar envio
                envToken envioWS = new envToken();
                envioWS.email = this._configuracaoIntegracaoTruckPad.UsuarioTruckPad;
                envioWS.password = this._configuracaoIntegracaoTruckPad.SenhaTruckPad;

                //Transmite o arquivo
                var retornoWS = this.TransmitirTruckPad(enumTipoWS.POST, envioWS, null, null, false, this._configuracaoIntegracaoTruckPad.URLTruckPadToken);

                //Processar Retorno
                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    mensagemErro = string.Concat("Ocorreu erro ao consumir o webservice: ", retornoWS.mensagem);

                    this.tokenAutenticacao = null;
                    return false;
                }
                else
                {
                    retToken retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retToken>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar token TruckPad JSON: {ex.ToString()}", "CatchNoAction");
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

                mensagemErro = "Ocorreu uma falha ao obter o tokem da TruckPad.";

                this.tokenAutenticacao = null;
                return false;
            }
        }

        #endregion
    }
}
