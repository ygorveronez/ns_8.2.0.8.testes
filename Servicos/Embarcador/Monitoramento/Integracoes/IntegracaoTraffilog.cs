using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoTraffilog : Abstract.AbstractIntegracaoREST
    {

        #region Atributos privados

        private static IntegracaoTraffilog Instance;
        private static readonly string nameConfigSection = "Traffilog";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a trafilog

        private string token;
        private bool aplicarFuso = true;

        #endregion


        #region Construtor privado

        private const string KEY_APLICAR_FUSO = "AplicarFusoHorario";

        private IntegracaoTraffilog(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Traffilog, nameConfigSection, cliente) { }

        #endregion


        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoTraffilog GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoTraffilog(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        /**
         * Complementa configurações gerais
         */
        override protected void ComplementarConfiguracoes()
        {
            try
            {
                this.aplicarFuso = bool.Parse(ObterValorOpcao(KEY_APLICAR_FUSO));
            }
            catch (Exception e)
            {
                Log($"Erro ao ComplementarConfiguracoes {e.Message}", 2);
            }
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
                Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.ResponsePosicoes retorno = ObterUltimasPosicoes();
                int total = retorno.response.properties.data.Count;
                Log($"Recebidas {total} ultimas posicoes de veiculos", 3);

                if (total > 0)
                {
                    for (int i = 0; i < total; i++)
                    {
                        var posicaoRecebida = retorno.response.properties.data[i];
                        DateTime dataPosicao = DateTime.Parse(posicaoRecebida.last_position_time.Replace("%3A", ":"));

                        if (dataPosicao != DateTime.MinValue)
                        {
                            posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                            {
                                Data = this.aplicarFuso ? dataPosicao.AddHours(-3) : dataPosicao,
                                DataVeiculo = this.aplicarFuso ? dataPosicao.AddHours(-3) : dataPosicao,
                                IDEquipamento = posicaoRecebida.vehicle_id,
                                Placa = posicaoRecebida.license_nmbr.Contains("*") ? posicaoRecebida.license_nmbr.Split('*')[1] : posicaoRecebida.license_nmbr,
                                Latitude = double.Parse(posicaoRecebida.latitude.Replace(".", ",")),
                                Longitude = double.Parse(posicaoRecebida.longtitude.Replace(".", ",")),
                                Velocidade = (int)double.Parse(posicaoRecebida.speed.Replace(".", ",")),
                                //Ignicao = (retorno.objetoRastreavelPosicaoAg[i].ultimaPosicao.estadoIgnicao == "ON") ? 1 : 0,
                                //Temperatura = retorno.objetoRastreavelPosicaoAg[i].temperature1,
                                SensorTemperatura = false,
                                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Traffilog
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
        private Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.ResponsePosicoes ObterUltimasPosicoes()
        {
            // Extrai os números únicos dos equipamento
            Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.RequestPosicoes request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.RequestPosicoes();
            List<string> numerosEquipamentos = base.ObterNumerosEquipamentosDosVeiculos();
            request.action = new Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.ActionRequestPosicoes();
            request.action.name = "api_get_data";
            request.action.session_token = this.token;

            string jsonRequestBody = JsonConvert.SerializeObject(request, Formatting.Indented);

            string bodyResponseUltimaPosicao = Request(jsonRequestBody, this.token);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.ResponsePosicoes retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.ResponsePosicoes();
            retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.ResponsePosicoes>(bodyResponseUltimaPosicao);

            return retorno;
        }

        /**
         * Autentica e obtém o token para as demais requisições
         */
        private void ObterToken()
        {
            Log("Obtendo Token", 2);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.RequestToken handshake = new Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.RequestToken();
            handshake.action = new Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.ActionRequestToken();
            handshake.action.parameters = new Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.ParametersRequestToken();
            handshake.action.name = "user_login";
            handshake.action.parameters.login_name = this.conta.Usuario;
            handshake.action.parameters.password = this.conta.Senha;

            string jsonRequestBody = JsonConvert.SerializeObject(handshake, Formatting.Indented);

            // Request
            string bodyResponseToken = Request(jsonRequestBody, null);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.ResponseToken responseHandshake = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog.ResponseToken>(bodyResponseToken);

            this.token = responseHandshake.response.properties.session_token;
        }



        // * Executa a requisição ao WebService
        // */
        private string Request(string body = null, string token = "")
        {
            string response = "";

            string url = ObterUrl("");

            // Requisição
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTraffilog));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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