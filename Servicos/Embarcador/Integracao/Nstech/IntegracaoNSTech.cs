using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Nstech
{
    public class IntegracaoNSTech
    {

        #region "Metodos protegidos"

        protected string ObterTokenIntegracaoNSTech(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech configuracaoIntegracao)
        {
            HttpClient client = ObterClient(configuracaoIntegracao.UrlAutenticacao);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Autenticacao autenticacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Autenticacao
            {
                ID = configuracaoIntegracao.IDAutenticacao,
                Senha = configuracaoIntegracao.SenhaAutenticacao
            };

            string jsonRequest = JsonConvert.SerializeObject(autenticacao, Formatting.Indented);
            var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

            var result = client.PostAsync(configuracaoIntegracao.UrlAutenticacao, content).Result;
            string jsonResponse = result.Content.ReadAsStringAsync().Result;

            //Tratamento para seguir o redirect de requisições, quando o retorno for Permanent Redirect (308).
            if ((int)result.StatusCode == 308)
            {
                string redirectUrl = result.Headers.Location.ToString();
                content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpClient clientRedirect = ObterClient(redirectUrl);
                result = clientRedirect.PostAsync(redirectUrl, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;
            }

            if (result.IsSuccessStatusCode)
            {
                dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                if ((bool)objetoRetorno?.verify == true)
                {
                    if (!string.IsNullOrWhiteSpace((string)objetoRetorno?.token))
                        return objetoRetorno.token;
                    else
                        throw new ServicoException("Autenticação não retornou token.");
                }
                else
                    throw new ServicoException("Não foi possível obter Token " + objetoRetorno?.msg);
            }
            else
                throw new ServicoException("Não foi possível obter Token: StatusCode " + result.StatusCode.ToString());
        }

        protected HttpClient ObterClient(string endPoint)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoNSTech));

            requisicao.BaseAddress = new Uri(endPoint);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return requisicao;
        }

        #endregion
    }
}
