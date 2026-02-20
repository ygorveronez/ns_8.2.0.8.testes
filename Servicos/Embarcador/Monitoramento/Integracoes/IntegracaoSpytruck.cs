using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoSpytruck : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoSpytruck Instance;
        private static readonly string nameConfigSection = "SpyTruck";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Ravex

        private string token;
        private DateTime tokenExpiration;
        private DateTime ultimaDataConsultada;
        private DateTime dataAtual;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_ULTIMA_DATA = "UltimaData";
        private const string URI_HANDSHAKE = "ws/auth/login";
        private const string URI_ULTIMA_POSICAO = "ws/v1/tracking/receiver";
        #endregion

        #region Construtor privado

        private IntegracaoSpytruck(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Spytruck, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoSpytruck GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoSpytruck(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        /**
         * Complementa configurações gerais
         */
        override protected void ComplementarConfiguracoes()
        {

        }

        /**
         * Confirmação de parâmetros corretos, executada apenas uma vez
         */
        override protected void Validar()
        {

        }

        /**
         * Preparação para iniciar a execução, executada apenas uma vez
         */
        override protected void Preparar()
        {
            this.dataAtual = DateTime.Now;
        }

        /**
         * Execução principal de cada iteração da thread, repetida infinitamente
         */
        override protected void Executar(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;
            ObtemOuRevalidaToken();
            IntegrarPosicao();
        }

        #endregion

        #region Métodos privados

        /**
         * Executa a integração das posições, consultando no WebService e registrando na tabela T_POSICAO
         */
        private void IntegrarPosicao()
        {
            Log($"Consultando posicoes", 2);

            // Busca as posições do WebService
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            // Registra as posições recebidas dos veículos
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        /**
         * Busca as posições de todos os veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            if (TokenEstaValido())
            {
                try
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.SpyTruck.ResponseGetPosicoes> veiculos = ObterUltimasPosicoes();
                    int total = veiculos.Count;
                    Log($"Recebidas {total} ultimas posicoes de veiculos", 3);

                    if (total > 0)
                    {
                        for (int i = 0; i < total; i++)
                        {
                            posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                            {
                                Data = DateTime.Parse(veiculos[i].dhTracking).AddHours(-3),
                                DataVeiculo = DateTime.Parse(veiculos[i].dhTracking).AddHours(-3),
                                IDEquipamento = veiculos[i].idVeiculo.ToString(),
                                Placa = veiculos[i].licensePlate.Trim().Replace("-", "").ToUpper(),
                                Latitude = veiculos[i].latitude,
                                Longitude = veiculos[i].longitude,
                                Velocidade = veiculos[i].speed,
                                Ignicao = (veiculos[i].trip) ? 1 : 0,
                                Temperatura = veiculos[i].temperature1,
                                SensorTemperatura = true,
                                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.SpyTruck
                            });
                        }

                        Log($"{posicoes.Count} posicoes", 3);
                    }
                }
                catch (Exception ex)
                {
                    Log("Erro ObterUltimasPosicoes " + ex.Message, 3);
                }
            }
            else
            {
                Log("Token invalido ", 3);
            }
            return posicoes;
        }


        /**
         * Requisição ao servico "posicoes/ultimaPosicao"
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.SpyTruck.ResponseGetPosicoes> ObterUltimasPosicoes()
        {
            string bodyResponseUltimaPosicao = Request(URI_ULTIMA_POSICAO, null, this.token);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.SpyTruck.ResponseGetPosicoes> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SpyTruck.ResponseGetPosicoes>();
            posicoes = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.SpyTruck.ResponseGetPosicoes>>(bodyResponseUltimaPosicao);

            return posicoes;
        }

        /**
         * Autentica e obtém o token para as demais requisições
         */
        private void ObterToken()
        {
            Log("Obtendo Token", 2);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            Dominio.ObjetosDeValor.Embarcador.Integracao.SpyTruck.Handshake handshake = new Dominio.ObjetosDeValor.Embarcador.Integracao.SpyTruck.Handshake();
            handshake.username = this.conta.Usuario;
            handshake.password = this.conta.Senha;
            string jsonRequestBody = JsonConvert.SerializeObject(handshake, Formatting.Indented);

            // Request
            string bodyResponseHandshake = Request(URI_HANDSHAKE, jsonRequestBody, null);
            Dominio.ObjetosDeValor.Embarcador.Integracao.SpyTruck.ResponseHandshake responseHandshake = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.SpyTruck.ResponseHandshake>(bodyResponseHandshake);

            this.token = responseHandshake.id_token;
            DateTime dataToken = DateTime.MinValue;
            DateTime.TryParseExact(responseHandshake.expiration, "dd/MM/yyyy HH:mm:ss", cultura, DateTimeStyles.None, out dataToken);

            if (dataToken != DateTime.MinValue)
                this.tokenExpiration = dataToken;
            else
                this.tokenExpiration = DateTime.Now.AddHours(3);
        }

        /**
         * Confirma que o token ainda está valido
         */
        private void ObtemOuRevalidaToken()
        {
            if (!TokenEstaValido()) ObterToken();
        }

        /**
         * Verifica a validadedo token
         */
        private bool TokenEstaValido()
        {
            return (!string.IsNullOrWhiteSpace(this.token) && this.tokenExpiration > DateTime.Now);
        }

        /**
         * Executa a requisição ao WebService
         */
        private string Request(string uri, string body = null, string token = "")
        {
            string response = "";

            string url = ObterUrl(uri);

            // Requisição
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSpytruck));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Execução da reequisição
            DateTime inicio = DateTime.UtcNow;
            Log($"Requisicao {uri}", inicio, 3);

            StringContent content;

            if (!string.IsNullOrEmpty(body))
            {
                content = new StringContent(body, Encoding.UTF8, "application/json");
                var result = client.PostAsync(url, content).Result;

                // Leitura da resposta
                response = result.Content.ReadAsStringAsync().Result;

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
            }
            else
            {
                var result = client.GetAsync(url).Result;

                // Leitura da resposta
                response = result.Content.ReadAsStringAsync().Result;

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
            }

            return response;
        }

        /**
         * Cria a URL para a requisição
         */
        private string ObterUrl(string uri)
        {
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);

            if (!string.IsNullOrWhiteSpace(uri))
                url += uri;

            return url;
        }


        #endregion

    }

}
