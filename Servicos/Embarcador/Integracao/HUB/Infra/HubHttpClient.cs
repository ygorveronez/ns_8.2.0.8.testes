using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.HUB.Infra
{
    public sealed class HubHttpClient
    {
        private readonly Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHUB _configuracao;
        private readonly HttpClient _http;

        private string? _accessToken;
        private DateTimeOffset _accessTokenExpiresAt;

        public HubHttpClient(
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHUB configuracaoIntegracaoHUB,
            HttpClient? httpClient = null)
        {
            _configuracao = configuracaoIntegracaoHUB ?? throw new ArgumentNullException(nameof(configuracaoIntegracaoHUB));

            _http = httpClient ?? new HttpClient();

            if (!string.IsNullOrWhiteSpace(_configuracao.UrlIntegracao))
                _http.BaseAddress = new Uri(NormalizeBaseUrl(_configuracao.UrlIntegracao));
        }

        public async Task<HttpRequisicaoResposta> GetAsync(string endpoint, CancellationToken ct = default)
        {
            var token = await GetBearerTokenAsync(ct);

            using var request = new HttpRequestMessage(HttpMethod.Get, NormalizeEndpoint(endpoint));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _http.SendAsync(request, ct);
            var responseBody = await response.Content.ReadAsStringAsync();

            return new HttpRequisicaoResposta
            {
                conteudoRequisicao = endpoint,
                conteudoResposta = responseBody,
                httpStatusCode = response.StatusCode
            };
        }

        public async Task<HttpRequisicaoResposta> PostAsync(string endpoint, object dados, CancellationToken ct = default)
        {
            var token = await GetBearerTokenAsync(ct);

            var settings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = "yyyy-MM-ddTHH:mm:sszzz"
            };

            string json = JsonConvert.SerializeObject(dados, settings);

            using var request = new HttpRequestMessage(HttpMethod.Post, NormalizeEndpoint(endpoint));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _http.SendAsync(request, ct);
            var responseBody = await response.Content.ReadAsStringAsync();

            return new HttpRequisicaoResposta
            {
                conteudoRequisicao = json,
                conteudoResposta = responseBody,
                httpStatusCode = response.StatusCode
            };
        }

        private async Task<string> GetBearerTokenAsync(CancellationToken ct)
        {
            if (!string.IsNullOrWhiteSpace(_accessToken) && DateTimeOffset.UtcNow < _accessTokenExpiresAt)
                return _accessToken!;

            var tokenUrl = _configuracao.UrlAutenticacaoToken;
            if (string.IsNullOrWhiteSpace(tokenUrl))
                throw new InvalidOperationException("UrlAutenticacaoToken não configurada.");

            var autenticacao = new Dictionary<string, string>
            {
                { "client_secret", _configuracao.ChaveSecreta },
                { "grant_type", "client_credentials" },
                { "client_id", _configuracao.IdCliente }
            };

            using var content = new FormUrlEncodedContent(autenticacao);
            using var response = await _http.PostAsync(tokenUrl, content, ct);
            var jsonResposta = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erro ao autenticar: " + jsonResposta);

            var obj = JObject.Parse(jsonResposta);
            var token = obj.Value<string>("access_token");

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Token não retornado pelo endpoint de autenticação: " + jsonResposta);

            var expiresInSeconds = obj.Value<int?>("expires_in") ?? 3000;

            _accessToken = token;
            _accessTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds - 30);

            return token;
        }


        private static string NormalizeBaseUrl(string url)
        {
            return url.EndsWith("/") ? url : url + "/";
        }

        private static string NormalizeEndpoint(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint)) return "";
            return endpoint.StartsWith("/") ? endpoint.Substring(1) : endpoint;
        }
    }
}
