using Dominio.ObjetosDeValor.Embarcador.Logistica;
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
    public class IntegracaoRotaExata : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoRotaExata Instance;
        private static readonly string nameConfigSection = "RotaExata";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        private const string URI_HANDSHAKE = "login";
        private const string URI_ULTIMA_POSICAO = "ultima-posicao/todos";

        #endregion

        #region Construtor privado

        private IntegracaoRotaExata(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.RotaExata, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoRotaExata GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoRotaExata(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        protected override void ComplementarConfiguracoes() { }
        protected override void Validar() { }
        protected override void Preparar() { }

        protected override void Executar(ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;
            IntegrarPosicao();
        }

        #endregion

        #region Métodos privados 

        private void IntegrarPosicao()
        {
            Log($"Consultando posicoes", 2);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.ResponseGetPosicoes posicoesRecebidas = BuscaUltimaPosicaoVeiculos();

                if (posicoesRecebidas != null && posicoesRecebidas.data.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.Datum ultimaPosicaoVeiculo in posicoesRecebidas.data)
                        posicoes.Add(ObtemPosicao(ultimaPosicaoVeiculo));

                    Log($"Recebidas posiçoes de {posicoes.GroupBy(x => x.Placa).ToList().Count} veículos", 3);
                }

                Log($"{posicoes.Count} posicoes", 3);

            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
                Log("Erro ObterPosicoes " + ex.InnerException.Message, 3);
            }

            return posicoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.ResponseGetPosicoes BuscaUltimaPosicaoVeiculos()
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.ResponseHandshake dadosToken = ObterToken();

            if (!string.IsNullOrEmpty(dadosToken.token))
            {
                string bodyResponseUltimaPosicao = Request(URI_ULTIMA_POSICAO, null, dadosToken.token);

                Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.ResponseGetPosicoes posicoes = new Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.ResponseGetPosicoes();
                posicoes = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.ResponseGetPosicoes>(bodyResponseUltimaPosicao);

                return posicoes;

            }
            else
            {
                return new Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.ResponseGetPosicoes();
            }
        }


        private Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.ResponseHandshake ObterToken()
        {
            Log("Obtendo Token", 2);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.Handshake handshake = new Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.Handshake();
            handshake.email = this.conta.Usuario;
            handshake.password = this.conta.Senha;
            handshake.expires = 86400;
            string jsonRequestBody = JsonConvert.SerializeObject(handshake, Formatting.Indented);

            // Request
            string bodyResponseHandshake = Request(URI_HANDSHAKE, jsonRequestBody, null);
            Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.ResponseHandshake responseHandshake = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.ResponseHandshake>(bodyResponseHandshake);

            return responseHandshake;
        }

        private string Request(string uri, string body = null, string token = "")
        {
            string response = "";
            string url = ObterUrl(uri);

            //Requisição
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoRotaExata));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Add("Authorization", token);

            //Execução da reequisição
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

        private Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao ObtemPosicao(Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata.Datum posicaoRota)
        {
            DateTime dataVeiculo = DateTime.Now;
            try
            {
                dataVeiculo = posicaoRota.posicao.dt_posicao.AddHours(-3); //.ToDateTime(DateTime.Now);
            }
            catch (Exception e) 
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao converter data de posição RotaExata - continuando processamento: {e.ToString()}", "CatchNoAction");
            }; //Se não conseguiu converter a data, segue o baile.

            return new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
            {
                Data = dataVeiculo,
                DataCadastro = DateTime.Now,
                DataVeiculo = dataVeiculo,
                IDEquipamento = posicaoRota.posicao.rastreador_id.ToString(),
                Placa = posicaoRota.posicao.adesao.vei_placa.Replace("-", ""),
                Latitude = posicaoRota.posicao.latitude,
                Longitude = posicaoRota.posicao.longitude,
                Velocidade = posicaoRota.posicao.velocidade ?? 0,
                Temperatura = 0,
                SensorTemperatura = false,
                Descricao = "",
                NivelBateria = 0,
                Ignicao = posicaoRota.posicao.parado_ligado ?? 0,
                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.RotaExata
            };
        }

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

