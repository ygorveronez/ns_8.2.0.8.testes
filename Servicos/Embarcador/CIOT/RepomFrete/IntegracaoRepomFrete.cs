using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;
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

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete configuracaoIntegracaoRepomFrete = null;
        string tokenAutenticacao = null;

        #region Métodos Globais

        #endregion

        #region Métodos Privados

        public retornoWebService TransmitirRepom(enumTipoWS tipoWS, object objEnvio, string metodo, string token, bool formUrlEncoded = false, string version = "2.2")
        {
            return this.Transmitir(tipoWS, null, objEnvio, metodo, token, formUrlEncoded, version);
        }

        public retornoWebService TransmitirRepom(enumTipoWS tipoWS, string parametroGET, string metodo, string token, string version = "2.2")
        {
            return this.Transmitir(tipoWS, parametroGET, null, metodo, token, true, version);
        }

        private retornoWebService Transmitir(enumTipoWS tipoWS, string parametroGET, object objEnvio, string metodo, string token, bool formUrlEncoded = false, string version = "2.2")
        {
            var retornoWS = new Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete.retornoWebService();

            try
            {
                if (configuracaoIntegracaoRepomFrete == null )
                    throw new ServicoException("Processo Abortado! Integração não configurada.");

                if (string.IsNullOrEmpty(configuracaoIntegracaoRepomFrete.URLRepomFrete))
                    throw new ServicoException("Processo Abortado! URL não definida.");

                string url = null;
                if (configuracaoIntegracaoRepomFrete.URLRepomFrete.EndsWith("/"))
                    url = configuracaoIntegracaoRepomFrete.URLRepomFrete;
                else
                    url = configuracaoIntegracaoRepomFrete.URLRepomFrete + "/";
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

                url += $"?x-api-version={version}";

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoRepomFrete));

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

        private Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete ObterConfiguracaoRepomFrete(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.CIOT.CIOTRepomFrete repCIOTRepom = new Repositorio.Embarcador.CIOT.CIOTRepomFrete(unidadeTrabalho);

            Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete configuracao = null;
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
                if (configuracaoIntegracaoRepomFrete == null)
                    throw new ServicoException("Processo Abortado! Integração não configurada.");

                //Caso a senha e usuario não estiverem preenchidas a autenticação será através do certificado digital;
                if (string.IsNullOrEmpty(this.configuracaoIntegracaoRepomFrete.SenhaRepomFrete) || string.IsNullOrEmpty(this.configuracaoIntegracaoRepomFrete.UsuarioRepomFrete))
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
                envioWS.grant_type = "password";
                envioWS.username = this.configuracaoIntegracaoRepomFrete.UsuarioRepomFrete;
                envioWS.password = this.configuracaoIntegracaoRepomFrete.SenhaRepomFrete;
                envioWS.partner = this.configuracaoIntegracaoRepomFrete.PartnerRepomFrete;

                //Transmite o arquivo
                var retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "token", null, true);

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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar token de autenticação RepomFrete: {ex.ToString()}", "CatchNoAction");
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
                            if (!string.IsNullOrEmpty(retorno.error_description))
                                mensagemErro = string.Format("Error: {0}; Message: {1}.", retorno.error, retorno.error_description);
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

        private retornoObterTelefone ObterTelefoneRepomFrete(string paissigla, string telefoneCadastro)
        {
            var retorno = new retornoObterTelefone();

            string ddd = null;
            string telefone = null;

            Regex buscarDDD = new Regex(@"^\((?<ddd>[0-9]{2})\) ?(?<fone>[0-9-]+)");
            var ret = buscarDDD.Match(telefoneCadastro);
            if (ret.Success)
            {
                ddd = ret.Groups["ddd"].Value;
                telefone = Regex.Replace(ret.Groups["fone"].Value, "[^0-9]", "");
            }
            else
            {
                string telefoneCompleto = Regex.Replace(telefoneCadastro, "[^0-9]", "");

                if (!string.IsNullOrEmpty(telefoneCompleto))
                {
                    if (telefoneCompleto.Length >= 10)
                    {
                        ddd = telefoneCompleto.Substring(0, 2);
                        telefone = telefoneCompleto.Substring(2, (telefoneCompleto.Length - 2));
                    }
                    else
                    {
                        ddd = null;
                        telefone = telefoneCompleto;
                    }
                }
            }

            if (!string.IsNullOrEmpty(telefone))
            {
                retorno.ddi = paissigla == "BRASIL" ? "55" : null;
                retorno.ddd = ddd;
                retorno.prefixo_mais_numero = telefone;

                if (telefone.Length >= 9)
                {
                    retorno.prefixo = telefone.Substring(0, 5);
                    retorno.numero = telefone.Substring(5, (telefone.Length - 5));
                }
                else if (telefone.Length >= 5)
                {
                    retorno.prefixo = telefone.Substring(0, 4);
                    retorno.numero = telefone.Substring(4, (telefone.Length - 4));
                }
                else
                {
                    retorno.prefixo = null;
                    retorno.numero = telefone;
                }
            }

            return retorno;
        }

        #endregion
    }
}

