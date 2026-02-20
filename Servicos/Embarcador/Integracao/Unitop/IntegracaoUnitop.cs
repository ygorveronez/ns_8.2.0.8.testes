using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Servicos.Embarcador.Integracao.Unitop
{
    public class IntegracaoUnitop
    {
        #region Atributos privados

        private static IntegracaoUnitop Instance;

        private string url;
        private string uriConsulta;
        private string solicitanteId;
        private string token;
        private string metodo;
        private string classe;

        #endregion

        #region Construtor privado

        private IntegracaoUnitop() { }

        #endregion

        #region Métodos públicos

        public static IntegracaoUnitop GetInstance()
        {
            if (Instance == null) Instance = new IntegracaoUnitop();
            return Instance;
        }

        public void SetURL(string url)
        {
            this.url = url;
        }

        public void SetSolicitanteId(string solicitanteId)
        {
            this.solicitanteId = solicitanteId;
        }

        public void SetToken(string token)
        {
            this.token = token;
        }

        public void SetClasse(string classe)
        {
            this.classe = classe;
        }

        public void SetMethod(string method)
        {
            this.metodo = method;
        }

        public void MontarUriConsulta()
        {
            if (!string.IsNullOrWhiteSpace(this.uriConsulta)) return;

            VerificarConfiguracoes();

            this.uriConsulta = $"?class={classe}&method={metodo}&id_cli_espelhamento={this.solicitanteId}";
        }

        public bool VerificarUrlConsulta()
        {
            if (string.IsNullOrWhiteSpace(this.uriConsulta))
                return false;

            return uriConsulta.Contains("class=") &&
                   uriConsulta.Contains("method=") &&
                   uriConsulta.Contains("id_cli_espelhamento=");
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unitop.Veiculo> BuscarLocalizacoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unitop.Veiculo> veiculos = new();

            if (string.IsNullOrWhiteSpace(this.uriConsulta))
                throw new ServicoException("URI de consulta Unitop não definida");

            try
            {
                using (HttpClient client = CriarRequisicao())
                {
                    string endpoint = $"{this.url}{this.uriConsulta}";
                    HttpResponseMessage response = client.GetAsync(endpoint).Result;

                    string json = response.Content.ReadAsStringAsync().Result;

                    if (!response.IsSuccessStatusCode)
                        throw new ServicoException($"Erro HTTP ({(int)response.StatusCode}): {json}");

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Unitop.LocalizacoesResponse resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Unitop.LocalizacoesResponse>(json);

                    veiculos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unitop.Veiculo>>(resposta.Data);
                }
            }
            catch (WebException ex)
            {
                string mensagemDetalhada = ObterMensagemDetalhadaErro(ex);
                throw new ServicoException(mensagemDetalhada);
            }

            return veiculos;
        }

        #endregion

        #region Métodos privados

        private HttpClient CriarRequisicao()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoUnitop));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(this.token))
                requisicao.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic " + this.token);

            return requisicao;
        }


        private string ObterMensagemDetalhadaErro(WebException ex)
        {
            if (ex.Response is HttpWebResponse errorResponse)
            {
                using var reader = new StreamReader(errorResponse.GetResponseStream());
                string errorBody = reader.ReadToEnd();

                try
                {
                    var erroApi = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Unitop.ErrorResponse>(errorBody);
                    string detalhes = !string.IsNullOrWhiteSpace(erroApi?.Detalhes) ? $" Detalhes: {erroApi.Detalhes}" : "";
                    return $"Erro HTTP {(int)errorResponse.StatusCode} : {erroApi?.Erro}.{detalhes}";
                }
                catch
                {
                    return $"Erro HTTP {(int)errorResponse.StatusCode} : {errorBody}";
                }
            }

            return "Erro de conexão com a API Unitop.";
        }

        private void VerificarConfiguracoes()
        {
            if (string.IsNullOrWhiteSpace(this.classe))
                throw new ServicoException("Classe não definida para consulta Unitop.");

            if (string.IsNullOrWhiteSpace(this.metodo))
                throw new ServicoException("Método não definido para consulta Unitop.");

            if (string.IsNullOrWhiteSpace(this.solicitanteId))
                throw new ServicoException("SolicitanteId (id_cli_espelhamento) não definido.");

            if (string.IsNullOrWhiteSpace(this.token))
                throw new ServicoException("O token não foi definido");

            if (string.IsNullOrWhiteSpace(this.url))
                throw new ServicoException("URL Unitop não definida");
        }

        #endregion
    }
}
