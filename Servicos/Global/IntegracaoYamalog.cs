using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos
{
    public class IntegracaoYamalog
    {
        public static void IntegrarMDFe(ref Dominio.Entidades.MDFeIntegracaoRetorno mdfeIntegracaoRetorno, string endPoint, string urlToken, string clientID, string clientSecret, Repositorio.UnitOfWork unitOfWork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                Repositorio.MDFeIntegracaoRetornoLog repMDFeIntegracaoRetornoLog = new Repositorio.MDFeIntegracaoRetornoLog(unitOfWork);
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumetosMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
                List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentosMDFe = repDocumetosMDFe.BuscarPorMDFe(mdfeIntegracaoRetorno.MDFe.Codigo);

                string token = ObterToken(urlToken, clientID, clientSecret);

                Dominio.ObjetosDeValor.Yamalog.RetornoMDFe retornoMDFe = new Dominio.ObjetosDeValor.Yamalog.RetornoMDFe()
                {
                    data = DateTime.Now.ToString("yyyy-MM-dd"),
                    tipo = "CONFIRMACAO",
                    chave = repDocumetosMDFe.BuscarChavesDeCTesPorMDFe(mdfeIntegracaoRetorno.MDFe.Codigo)
                };

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoYamalog));

                client.BaseAddress = new Uri(endPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                jsonRequest = JsonConvert.SerializeObject(retornoMDFe, Formatting.Indented);
                var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);

                    dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);

                    if (objetoRetorno.status_code == "200" || objetoRetorno.status_code == "201")
                        mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Sucesso;
                    else
                        mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;

                    mdfeIntegracaoRetorno.DataIntegracao = DateTime.Now;
                    mdfeIntegracaoRetorno.ProblemaIntegracao = objetoRetorno.message;
                    mdfeIntegracaoRetorno.NumeroTentativas += 1;
                    mdfeIntegracaoRetorno.DataIntegracao = DateTime.Now;

                    Dominio.Entidades.MDFeIntegracaoRetornoLog mdfeIntegracaoRetornoLog = new Dominio.Entidades.MDFeIntegracaoRetornoLog();
                    mdfeIntegracaoRetornoLog.MDFeIntegracaoRetorno = mdfeIntegracaoRetorno;
                    mdfeIntegracaoRetornoLog.Data = DateTime.Now;
                    mdfeIntegracaoRetornoLog.Mensagem = objetoRetorno.message;
                    mdfeIntegracaoRetornoLog.Request = jsonRequest;
                    mdfeIntegracaoRetornoLog.Response = jsonResponse;
                    repMDFeIntegracaoRetornoLog.Inserir(mdfeIntegracaoRetornoLog);

                }
                else
                {
                    mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    mdfeIntegracaoRetorno.ProblemaIntegracao = "Não foi possível integrar, verifique os arquivos de integração.";
                    mdfeIntegracaoRetorno.NumeroTentativas += 1;
                    mdfeIntegracaoRetorno.DataIntegracao = DateTime.Now;

                    Dominio.Entidades.MDFeIntegracaoRetornoLog mdfeIntegracaoRetornoLog = new Dominio.Entidades.MDFeIntegracaoRetornoLog();
                    mdfeIntegracaoRetornoLog.MDFeIntegracaoRetorno = mdfeIntegracaoRetorno;
                    mdfeIntegracaoRetornoLog.Data = DateTime.Now;
                    mdfeIntegracaoRetornoLog.Mensagem = "Não foi possível integrar, verifique os arquivos de integração.";
                    mdfeIntegracaoRetornoLog.Request = jsonRequest;
                    mdfeIntegracaoRetornoLog.Response = jsonResponse;
                    repMDFeIntegracaoRetornoLog.Inserir(mdfeIntegracaoRetornoLog);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("IntegrarMDFe: " + ex, "IntegracaoYamalog");
                if (jsonRequest != null)
                    Servicos.Log.TratarErro(jsonRequest, "IntegracaoYamalog");
                if (jsonResponse != null)
                    Servicos.Log.TratarErro(jsonResponse, "IntegracaoYamalog");

                mdfeIntegracaoRetorno.NumeroTentativas += 1;
                mdfeIntegracaoRetorno.ProblemaIntegracao = "Falha ao retornar integração";
                mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
            }

        }


        private static string ObterToken(string url, string id, string secret)
        {
            string requestUri = $"{url}?client_id={id}&client_secret={secret}&grant_type=client_credentials";
            string basicAuthorization = Utilidades.String.Base64Encode(id + ":" + secret); 

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoYamalog));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthorization);

            HttpResponseMessage result = client.PostAsync(requestUri, null).Result;

            if (!result.IsSuccessStatusCode)
                return null;

            string jsonResponse = result.Content.ReadAsStringAsync().Result;

            dynamic obj = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            return (string)obj.access_token;

        }

    }
}
