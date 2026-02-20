using Infrastructure.Services.HttpClientFactory;
using System;
using System.Net;
using System.Net.Http;

namespace Servicos.Embarcador.Integracao.META
{
    public class META
    {
        public HttpClient CriarRequisicaoConexaoMeta(string url, string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(META));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            return requisicao;
        }
    }
}
