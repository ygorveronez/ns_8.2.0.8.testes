using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento.Integracoes.Abstract
{
    public abstract class AbstractIntegracaoMonitoramentoREST : AbstractIntegracaoREST
    {
        protected Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;
        private string AuthScheme;

        protected AbstractIntegracaoMonitoramentoREST(
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, 
            string configSection, 
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente,
            string authScheme) : base(tipo, configSection, cliente)
        {
            AuthScheme = authScheme;
        }

        protected virtual async Task<TResponse> PostJson<TRequest, TResponse, TErrorResponse>(string metodo, TRequest payload, string token, int timeoutEmMinutos)
        {
            return await PostJson<TRequest, TResponse, TErrorResponse>(metodo, payload, token, timeoutEmMinutos, false);
        }

        protected virtual async Task<TResponse> PostJson<TRequest, TResponse, TErrorResponse>(string metodo, TRequest payload, string token, int timeoutEmMinutos, bool semVersionamento)
        {
            HttpClient cliente = HttpClientFactoryWrapper.GetClient(nameof(tipoIntegracao));

            if (timeoutEmMinutos > 0)
                cliente.Timeout = TimeSpan.FromMinutes(timeoutEmMinutos);

            string versao = semVersionamento ? string.Empty : ExtrairParametroAdicional("Versao");
            string url = MontarUrl(metodo, versao);

            if (token != null)
                cliente.DefaultRequestHeaders.Add("Authorization", FormataToken(token));

            var content = new StringContent(payload != null ? JsonSerializer.Serialize(payload) : null, Encoding.UTF8, "application/json");

            HttpResponseMessage result = await cliente.PostAsync(url, content);

            if (!result.IsSuccessStatusCode)
            {
                TErrorResponse errorResponse = default;
                string error = await result.Content.ReadAsStringAsync();

                try
                {
                    errorResponse = JsonSerializer.Deserialize<TErrorResponse>(
                        error, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch (Exception ex)
                {
                    throw new ServicoException($"Erro ao chamar o método {metodo}: {result.StatusCode}");
                }

                throw new ServicoException($"Erro ao chamar o método {metodo}: {result.StatusCode}: {errorResponse}");
            }

            string responseContent = await result.Content.ReadAsStringAsync();

            if (typeof(TResponse) == typeof(string))
                return (TResponse)(object)responseContent;

            return JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private string MontarUrl(string metodo, string versao)
        {
            string url = $"{Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProtocoloHelper.ObterValor(conta.Protocolo)}://{conta.Servidor}";


            if (!string.IsNullOrWhiteSpace(versao))
                url += $"/{versao}";

            url += $"/{metodo}";

            return url;
        }

        private string ExtrairParametroAdicional(string chave)
        {
            if (conta.ListaParametrosAdicionais == null)
                return string.Empty;

            KeyValuePair<string, string> param = conta.ListaParametrosAdicionais.Find(p => p.Key == chave);

            return param.Key != null ? param.Value : string.Empty;
        }

        private string FormataToken(string token)
        {
            if (string.IsNullOrWhiteSpace(AuthScheme))
                return token;
            
            return $"{AuthScheme} {token}";
        }
    }
}
