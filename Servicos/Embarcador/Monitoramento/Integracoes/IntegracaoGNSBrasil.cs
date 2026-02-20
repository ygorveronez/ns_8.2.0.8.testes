using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoGNSBrasil : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoGNSBrasil Instance;
        private static readonly string nameConfigSection = "GNSBrasil";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;
        private const string KEY_TOKEN = "HashAuth";
        #endregion

        #region Construtor privado

        private IntegracaoGNSBrasil(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GNSBrasil, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoGNSBrasil GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoGNSBrasil(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        protected override void ComplementarConfiguracoes()
        {

        }
        protected override void Validar()
        {
            base.ValidarConfiguracaoDiretorioEArquivoControle(base.contasIntegracao);
        }
        protected override void Preparar()
        {

        }

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
            Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.ResponseToken Token = ObterToken();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.PositionResult> retornoRequisicao = BuscarPosicoesVeiculos(Token.AccessToken);

                Log($"Recebidas posiçoes de {retornoRequisicao.Count} veículos", 3);

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.PositionResult p in retornoRequisicao)
                {
                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {

                        Data = p.UpdateDate,
                        DataCadastro = DateTime.Now,
                        DataVeiculo = p.EventDate,
                        IDEquipamento = p.IdTrackedUnit.ToString(),
                        Placa = p.TrackedUnit.Replace("-", "").Substring(0, 7),
                        Latitude = (double)p.Latitude,
                        Longitude = (double)p.Longitude,
                        Velocidade = 0,
                        Temperatura = 0,
                        SensorTemperatura = false,
                        Descricao = "",
                        NivelBateria = 0,
                        Ignicao = p.Ignition == true ? 1 : 0,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.GNSBrasil
                    });

                }

                Log($"{posicoes.Count} posicoes", 3);
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.PositionResult> BuscarPosicoesVeiculos(string Token)
        {
            string url = $"{conta.Protocolo}://{conta.Servidor}/Tracking/PositionHistory/List";

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.RequestPosition> positionRequest = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.RequestPosition>();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.PositionResult> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.PositionResult>();
            var dataHoraReferencia = DateTime.Now.AddHours(-1);

            positionRequest.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.RequestPosition
            {
                Condition = "GreaterThan",
                Value = dataHoraReferencia.ToString("yyyy-MM-ddThh:mm:ss.00Z"),
                PropertyName = "EventDate"
            });

            dataHoraReferencia = dataHoraReferencia.AddHours(1);
            positionRequest.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.RequestPosition
            {
                Condition = "LessThan",
                Value = dataHoraReferencia.ToString("yyyy-MM-ddThh:mm:ss.00Z"),
                PropertyName = "EventDate"
            });

            string jsonRequestBody = JsonConvert.SerializeObject(positionRequest, Formatting.Indented);
            string bodyResponseHandshake = RequestAsync(url, Token, jsonRequestBody).Result;
            var posicoesRecebidas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.PositionResult>>(bodyResponseHandshake);

            posicoes.AddRange(posicoesRecebidas);

            return posicoes;
        }


        private Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.ResponseToken ObterToken()
        {
            Log("Obtendo Token", 2);

            string url = $"{conta.Protocolo}://{conta.Servidor}/{conta.URI}";

            var bodyResponseHandshake = RequestBearerToken(url).Result;

            Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.ResponseToken responseHandshake = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil.ResponseToken>(bodyResponseHandshake);

            return responseHandshake;
        }


        private async Task<string> RequestBearerToken(string url)
        {
            var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoGNSBrasil));
            var request = new HttpRequestMessage(HttpMethod.Post, $"{url}?Username={this.conta.Usuario}&Password={this.conta.Senha}&HashAuth={ObterHashAuth()}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var retorno = await response.Content.ReadAsStringAsync();
            return retorno;

            //var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoGNSBrasil));
            //var request = new HttpRequestMessage(HttpMethod.Post, url);
            //var content = new MultipartFormDataContent();
            //content.Add(new StringContent(this.conta.Usuario), "Username");
            //content.Add(new StringContent(this.conta.Senha), "password");
            //content.Add(new StringContent(ObterHashAuth()), "HashAuth");
            //request.Content = content;
            //var response = await client.SendAsync(request);
            //response.EnsureSuccessStatusCode();

            //var retorno = await response.Content.ReadAsStringAsync();
            //return retorno;
        }


        private async Task<string> RequestAsync(string url, string token = null, string jsonBody = null)
        {
            var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoGNSBrasil));
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", token);
            var content = new StringContent(jsonBody, null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var ret = await response.Content.ReadAsStringAsync();
            return ret;
        }

        private string ObterHashAuth()
        {
            try
            {
                return Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_TOKEN, this.conta.ListaParametrosAdicionais);
            }
            catch
            {
                return "";
            }
        }
        #endregion
    }
}
