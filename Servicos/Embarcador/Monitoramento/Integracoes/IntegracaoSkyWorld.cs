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
    public class IntegracaoSkyWorld : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoSkyWorld Instance;
        private static readonly string nameConfigSection = "SkyWorld";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;
        private bool aplicarFuso = true;

        #endregion

        #region Construtor privado

        private const string KEY_APLICAR_FUSO = "AplicarFusoHorario";

        private IntegracaoSkyWorld(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SkyWorld, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoSkyWorld GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoSkyWorld(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        protected override void ComplementarConfiguracoes()
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
        protected override void Validar()
        {
            base.ValidarConfiguracaoDiretorioEArquivoControle(base.contasIntegracao);
        }
        protected override void Preparar()
        {

        }

        protected override void Executar(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
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
                var retornoRequisicao = BuscaUltimaPosicaoVeiculos();

                Log($"Recebidas posiçoes de {retornoRequisicao.Count} veículos", 3);

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld.RetornoPosicoes p in retornoRequisicao)
                {
                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {

                        Data = this.aplicarFuso ? DateTime.Parse(p.dt_server).AddHours(-3) : DateTime.Parse(p.dt_server),
                        DataVeiculo = this.aplicarFuso ? DateTime.Parse(p.dt_tracker).AddHours(-3) : DateTime.Parse(p.dt_tracker),
                        IDEquipamento = p.name,
                        Placa = p.plate_number,
                        Latitude = double.Parse(p.lat.Replace(".", ",")),
                        Longitude = double.Parse(p.lng.Replace(".", ",")),
                        Velocidade = int.Parse(p.speed),
                        Temperatura = 0,
                        SensorTemperatura = false,
                        Descricao = p.device,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.SkyWorld
                    });

                    Log($"{posicoes.Count} posicoes", 3);
                }

            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld.RetornoRequestToken ObterToken()
        {

            Log("Obtendo Token", 2);

            Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld.RequestToken handshake = new Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld.RequestToken();
            handshake.usuario = this.conta.Usuario;
            handshake.password = this.conta.Senha;

            string jsonRequestBody = JsonConvert.SerializeObject(handshake, Formatting.Indented);

            // Request
            string url = $"{conta.Protocolo}://{conta.Servidor}/api/app/auth-key.php";

            string bodyResponseHandshake = Request(url, jsonRequestBody);
            Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld.RetornoRequestToken responseHandshake = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld.RetornoRequestToken>(bodyResponseHandshake);

            return responseHandshake;
        }


        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld.RetornoPosicoes> BuscaUltimaPosicaoVeiculos()
        {
            var token = ObterToken();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld.RetornoPosicoes> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld.RetornoPosicoes>();

            if (token.Ok)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld.RequestPosicoes handshake = new Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld.RequestPosicoes();
                handshake.id = token.Result.Id;
                handshake.token = token.Result.Token;

                string jsonRequestBody = JsonConvert.SerializeObject(handshake, Formatting.Indented);

                string bodyResponseHandshake = Request("", jsonRequestBody);

                posicoes = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld.RetornoPosicoes>>(bodyResponseHandshake);
            }

            return posicoes;
        }


        private string Request(string uri, string body = null)
        {
            string response = "";

            string url = uri;
            if (string.IsNullOrEmpty(uri))
                url = ObterUrl(uri);

            // Requisição
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSkyWorld));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //if (!string.IsNullOrEmpty(token))
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Execução da reequisição
            DateTime inicio = DateTime.UtcNow;
            Log($"Requisicao {uri}", inicio, 3);

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
