using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoTransPanorama : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoTransPanorama Instance;
        private static readonly string nameConfigSection = "TransPanorama";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a CSX

        private string token;

        #endregion

        #region Construtor privado

        private IntegracaoTransPanorama(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TransPanorama, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoTransPanorama GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoTransPanorama(cliente);
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
            // retirar 
            //ObterToken();
        }

        /**
         * Execução principal de cada iteração da thread, repetida infinitamente
         */
        override protected void Executar(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {

            this.conta = contaIntegracao;
            ObterToken();
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
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.TransPanorama.ResponseLastPosition retorno = ObterUltimasPosicoes();
                int total = retorno.positions.Count();
                Log($"Recebidas {total} ultimas posicoes de veiculos", 3);

                if (total > 0)
                {
                    for (int i = 0; i < total; i++)
                    {
                        var posicaoRecebida = retorno.positions[i];
                        DateTime dataPosicao = DateTime.Parse(posicaoRecebida.dateGps);

                        if (dataPosicao != DateTime.MinValue)
                        {
                            posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                            {
                                Data = dataPosicao.AddHours(-3),
                                DataVeiculo = dataPosicao.AddHours(-3),
                                IDEquipamento = posicaoRecebida.deviceId.ToString(),
                                Placa = posicaoRecebida.identifier,
                                Latitude = double.Parse(posicaoRecebida.latitude.ToString().Replace(".", ",")),
                                Longitude = double.Parse(posicaoRecebida.longitude.ToString().Replace(".", ",")),
                                Velocidade = (int)double.Parse(posicaoRecebida.speed.ToString().Replace(".", ",")),
                                //Ignicao = (retorno.objetoRastreavelPosicaoAg[i].ultimaPosicao.estadoIgnicao == "ON") ? 1 : 0,
                                //Temperatura = retorno.objetoRastreavelPosicaoAg[i].temperature1,
                                SensorTemperatura = false,
                                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.TransPanorama
                            });
                        }
                    }

                    Log($"{total} posicoes", 3);
                }
            }
            catch (Exception ex)
            {
                Log("Erro ObterUltimasPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }

        /**
         * Requisição ao servico "posicoes/ultimaPosicao"
         */
        private Dominio.ObjetosDeValor.Embarcador.Integracao.TransPanorama.ResponseLastPosition ObterUltimasPosicoes()
        {
            // Extrai os números únicos dos equipamento
            List<string> numerosEquipamentos = base.ObterNumerosEquipamentosDosVeiculos();
            string bodyResponseUltimaPosicao = PosicoesRequest(this.token, "/report/lastPosition");

            Dominio.ObjetosDeValor.Embarcador.Integracao.TransPanorama.ResponseLastPosition retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.TransPanorama.ResponseLastPosition();
            retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.TransPanorama.ResponseLastPosition>(bodyResponseUltimaPosicao);

            return retorno;
        }

        /**
         * Autentica e obtém o token para as demais requisições
         */
        private void ObterToken()
        {
            Log("Obtendo Token", 2);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
            Dominio.ObjetosDeValor.Embarcador.Integracao.TransPanorama.RequestToken RequestToken = new Dominio.ObjetosDeValor.Embarcador.Integracao.TransPanorama.RequestToken()
            {
                client = "operacoes.transpanorama",
                login = this.conta.Usuario,
                psw = this.conta.Senha
            };
            string jsonRequestBody = JsonConvert.SerializeObject(RequestToken, Formatting.Indented);
            string bodyResponseToken = TokenRequest(jsonRequestBody, null, "/user/login");
            Dominio.ObjetosDeValor.Embarcador.Integracao.TransPanorama.ResponseToken responseToken = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.TransPanorama.ResponseToken>(bodyResponseToken);
            this.token = responseToken.token;
        }

        // * Executa a requisição ao WebService
        // */
        private string TokenRequest(string body = null, string token = "", string uri = "")
        {
            string response = "";

            string url = ObterUrl(uri);

            // Requisição
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTransPanorama));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("X-Auth-Token", token);

            // Execução da reequisição
            DateTime inicio = DateTime.UtcNow;
            Log($"Requisicao {url}", inicio, 3);

            StringContent content;

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


            return response;
        }

        private string PosicoesRequest(string token = "", string uri = "")
        {
            string response = "";

            string url = ObterUrl(uri);

            // Requisição
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTransPanorama));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(token))
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("X-Auth-Token", token);
                client.DefaultRequestHeaders.Add("X-Auth-Token", token);

            // Execução da reequisição
            DateTime inicio = DateTime.UtcNow;
            Log($"Requisicao {url}", inicio, 3);

            //StringContent content;

            //content = new StringContent(body, Encoding.UTF8, "application/json");
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