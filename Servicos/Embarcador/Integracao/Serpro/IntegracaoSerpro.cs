using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Serpro
{
    public class IntegracaoSerpro
    {

        public static bool Realizarlogin(Repositorio.UnitOfWork unitOfWork, out string msgRetorno, out string token, bool forcarAtualizarToken)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            msgRetorno = "";
            token = "";

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoSerpro || string.IsNullOrWhiteSpace(configuracaoIntegracao.ConsumerKeySerpro) || string.IsNullOrWhiteSpace(configuracaoIntegracao.ConsumerSecretSerpro) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLSerpro))
            {
                msgRetorno = "Não existe configuração de integração disponível para a Serpro.";
                return false;
            }

            if (configuracaoIntegracao.DataTokenSerpro.HasValue && !string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenSerpro) && !forcarAtualizarToken)
            {
                TimeSpan ts = DateTime.Now - configuracaoIntegracao.DataTokenSerpro.Value;
                if (ts.TotalMinutes <= 30)
                {
                    token = configuracaoIntegracao.TokenSerpro;
                    return true;
                }
            }

            string urlWebService = configuracaoIntegracao.URLSerpro;
            urlWebService += "/token";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSerpro));
            //Define Headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(configuracaoIntegracao.ConsumerKeySerpro + ":" + configuracaoIntegracao.ConsumerSecretSerpro)));

            //Prepare Request Body
            List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
            requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

            try
            {
                var result = client.PostAsync(urlWebService, requestBody).Result;
                if (result.IsSuccessStatusCode)
                {
                    var retorno = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);
                    token = (string)retorno.access_token;
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        configuracaoIntegracao.TokenSerpro = token;
                        configuracaoIntegracao.DataTokenSerpro = DateTime.Now;
                        repConfiguracaoIntegracao.Atualizar(configuracaoIntegracao);
                        return true;
                    }
                    else
                    {
                        msgRetorno = "Problemas ao comunicar com o serviço da SERPRO, favor tente novamente mais tarde, não retornou o token de acesso.";
                        return false;
                    }
                }
                else
                {
                    msgRetorno = "Problemas ao comunicar com o serviço da SERPRO, favor tente novamente mais tarde, falha ao tentar realizar o login. " + result.ReasonPhrase;
                    return false;
                }
            }
            catch (Exception ex)
            {
                token = "";
                msgRetorno = ex.Message;
                return false;
            }
        }

        public static string BaixarXMLPelaChave(Repositorio.UnitOfWork unitOfWork, out string msgRetorno, string token, string chave)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            msgRetorno = "";

            string urlWebService = configuracaoIntegracao.URLSerpro + "/consulta-nfe-df/api/v1/nfe/" + chave;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSerpro));
            //Define Headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var result = client.GetAsync(urlWebService).Result;
                if (result.IsSuccessStatusCode)
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        return retorno;
                    }
                    else
                    {
                        msgRetorno = "Problemas ao comunicar com o serviço da SERPRO, favor tente novamente mais tarde.";
                        return "";
                    }
                }
                else
                {
                    msgRetorno = "Problemas ao comunicar com o serviço da SERPRO, favor tente novamente mais tarde.";
                    return "";
                }
            }
            catch (Exception ex)
            {
                msgRetorno = ex.Message;
                return "";
            }
        }

        public static string BaixarJSONPelaChave(Repositorio.UnitOfWork unitOfWork, out string msgRetorno, string token, string chave)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            msgRetorno = "";

            string urlWebService = configuracaoIntegracao.URLSerpro + "/consulta-nfe-df/api/v1/nfe/" + chave;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSerpro));
            //Define Headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var result = client.GetAsync(urlWebService).Result;
                if (result.IsSuccessStatusCode)
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        return retorno;
                    }
                    else
                    {
                        msgRetorno = "Problemas ao comunicar com o serviço da SERPRO, favor tente novamente mais tarde, não retornou o objeto da NFe.";
                        return "";
                    }
                }
                else
                {
                    msgRetorno = "Problemas ao comunicar com o serviço da SERPRO, favor tente novamente mais tarde, falha ao tentar buscar o objeto da NFe. " + result.ReasonPhrase;
                    if (result.ReasonPhrase == "Unauthorized")
                        Servicos.Embarcador.Integracao.Serpro.IntegracaoSerpro.Realizarlogin(unitOfWork, out msgRetorno, out token, true);
                    return "";
                }
            }
            catch (Exception ex)
            {
                msgRetorno = ex.Message;
                return "";
            }
        }

    }

    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Servicos.Log.TratarErro("Request: " + request.ToString(), "IntegracaoSerpro");

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            Servicos.Log.TratarErro("Response: " + response.ToString(), "IntegracaoSerpro");

            return response;
        }
    }
}
