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
    public class IntegracaoViaLink : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoViaLink Instance;
        private static readonly string nameConfigSection = "ViaLink";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        private const string URI_ULTIMA_POSICAO = "/events/all/apiKey/{0}/secretKey/{1}";

        #endregion

        #region Construtor privado

        private IntegracaoViaLink(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ViaLink, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoViaLink GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoViaLink(cliente);
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
                Dominio.ObjetosDeValor.Embarcador.Integracao.ViaLink.ResponsePosicoes posicoesRecebidas = BuscaUltimaPosicaoVeiculos();

                if (posicoesRecebidas != null && posicoesRecebidas.data.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.ViaLink.TendenciaEntregaData ultimaPosicaoVeiculo in posicoesRecebidas.data)
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

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ViaLink.ResponsePosicoes BuscaUltimaPosicaoVeiculos()
        {


            string bodyResponseUltimaPosicao = Request(URI_ULTIMA_POSICAO, null);

            Dominio.ObjetosDeValor.Embarcador.Integracao.ViaLink.ResponsePosicoes posicoes = new();
            posicoes = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.ViaLink.ResponsePosicoes>(bodyResponseUltimaPosicao);

            return posicoes;

        }



        private string Request(string uri, string body = null, string token = "")
        {
            string response = "";
            string url = ObterUrl(uri);


            if (this.conta.ListaParametrosAdicionais.Count < 2)
                throw new Exception("Parâmetros de autenticação não configurados para vialink.");

            var apikey = this.conta.ListaParametrosAdicionais
                .FirstOrDefault(p => p.Key.Equals("apikey", StringComparison.OrdinalIgnoreCase))
                .Value;

            var secretkey = this.conta.ListaParametrosAdicionais
                .FirstOrDefault(p => p.Key.Equals("secretkey", StringComparison.OrdinalIgnoreCase))
                .Value;

            if (string.IsNullOrEmpty(apikey) || string.IsNullOrEmpty(secretkey))
                throw new Exception("Parâmetros de autenticação não configurados para vialink.");

            url = string.Format(url, apikey, secretkey);

            //Requisição
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoViaLink));
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

        private Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao ObtemPosicao(Dominio.ObjetosDeValor.Embarcador.Integracao.ViaLink.TendenciaEntregaData posicaoRota)
        {
            DateTime dataVeiculo = DateTime.Now;
            dataVeiculo = DateTime.Parse(posicaoRota.ras_ras_data_ult_comunicacao).ToLocalTime();

            return new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
            {
                Data = dataVeiculo,
                DataCadastro = DateTime.Now,
                DataVeiculo = dataVeiculo,
                IDEquipamento = posicaoRota.ras_ras_id,
                Placa = posicaoRota.ras_vei_placa.Replace("-", "").Replace(" ", ""),
                Latitude = double.TryParse(posicaoRota.ras_eve_latitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double result)
                ? result
                : 0,
                Longitude = double.TryParse(posicaoRota.ras_eve_longitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double resultLong)
                ? resultLong
                : 0,

                Velocidade = int.TryParse(posicaoRota.ras_eve_velocidade, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out int resultVelocidade)
                ? resultVelocidade
                : 0,

                Temperatura = 0,
                SensorTemperatura = false,
                Descricao = "",
                NivelBateria = 0,
                Ignicao = int.TryParse(posicaoRota.ras_eve_ignicao, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out int resultIgnicao)
                ? resultIgnicao
                : 0,

                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.ViaLink

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

