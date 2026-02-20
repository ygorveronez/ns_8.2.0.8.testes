using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Rastrear
{
    public class IntegracaoRastrear
    {
        #region Atributos privados

        public string token { get; set; }
        public DateTime validadeToken { get; set; }
        private static IntegracaoRastrear Instance;

        private Servicos.InspectorBehavior inspector;
        private string url;
        private string usuario;
        private string senha;

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora rastreadorId;

        #endregion

        #region Construtor privado

        private IntegracaoRastrear() { }

        #endregion

        #region Métodos públicos

        public static IntegracaoRastrear GetInstance()
        {
            if (Instance == null) Instance = new IntegracaoRastrear();
            return Instance;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Rastrear.AutReturn ObterToken()
        {
            VerificarConfiguracoes();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Rastrear.Aut handshakeAut = new Dominio.ObjetosDeValor.Embarcador.Integracao.Rastrear.Aut();
            handshakeAut.usuario = this.usuario;
            handshakeAut.senha = this.senha;
            string jsonRequestBody = JsonConvert.SerializeObject(handshakeAut, Formatting.Indented);

            // Request
            string jsonResponse = Request(jsonRequestBody, true);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Rastrear.AutReturn objetoRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Rastrear.AutReturn>(jsonResponse);
            return objetoRetorno;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Rastrear.ResponsePosicoes ObterPoscioesVeiculo()
        {
            // Request
            string jsonResponse = Request("", false);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Rastrear.ResponsePosicoes objetoRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Rastrear.ResponsePosicoes>(jsonResponse);
            return objetoRetorno;
        }


        public void DefinirConfiguracoes(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.ParametrosConfiguracaoRastreamento parametros)
        {
            if (!string.IsNullOrEmpty(parametros.url))
                this.url = parametros.url;

            if (!string.IsNullOrEmpty(parametros.usuario))
                this.usuario = parametros.usuario;

            if (!string.IsNullOrEmpty(parametros.senha))
                this.senha = parametros.senha;

            this.rastreadorId = parametros.rastreadorId.Value;
        }

        public void DefinirToken(string token)
        {
            this.token = token;
        }

        public void DefinirValidadeToken(DateTime data)
        {
            this.validadeToken = data;
        }
        #endregion

        #region Métodos privados

        private string Request(string body, bool token)
        {
            // Requisição
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoRastrear));
            client.BaseAddress = new Uri(this.url);
            client.DefaultRequestHeaders.Accept.Clear();

            if (!token && !string.IsNullOrEmpty(this.token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

            StringContent content;

            content = new StringContent(body, Encoding.UTF8, "application/json");
            HttpResponseMessage result;

            if (token)
                result = client.PostAsync(url, content).Result;
            else
                result = client.GetAsync(url).Result;

            // Leitura da resposta
            string response = result.Content.ReadAsStringAsync().Result;

            // Verificação do StatusCode
            switch (result.StatusCode)
            {
                case HttpStatusCode.OK: break;
                case HttpStatusCode.Unauthorized:
                    throw new Exception("Requer autenticação.");
                case HttpStatusCode.Forbidden:
                    throw new Exception("Acesso a requisição negada.");
                default:
                    throw new Exception("Erro na requisicao: HTTP Status " + result.StatusCode + " - " + result.RequestMessage);
            }

            return response;
        }
        private void VerificarConfiguracoes()
        {
            if (string.IsNullOrWhiteSpace(this.usuario)) throw new ServicoException("Usuario da conta acesso a posições Rastrear não definido");
            if (string.IsNullOrWhiteSpace(this.senha)) throw new ServicoException("Senha da conta acesso a posições Rastrear não definida");
        }

        #endregion

    }
}

