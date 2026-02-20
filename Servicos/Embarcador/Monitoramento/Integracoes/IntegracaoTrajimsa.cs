using Dominio.ObjetosDeValor.Embarcador.Integracao.TrustTrack;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoTrustTrack : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoTrustTrack Instance;
        private static readonly string nameConfigSection = "TrustTrack";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;
        private const string KEY_TOKEN = "api_key";
        private bool aplicarFuso = true;

        #endregion

        #region Construtor privado

        private const string KEY_APLICAR_FUSO = "AplicarFusoHorario";
        private IntegracaoTrustTrack(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrustTrack, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoTrustTrack GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoTrustTrack(cliente);
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
            List<Posicao> posicoes = new List<Posicao>();
            var token = ObterToken();
            if (token != "")
            {
                List<ObjectsRetorno> objetos = ObterObjetos(token);

                foreach (var obj in objetos)
                {
                    RetornoItems retorno = ObterItems(obj.Id, token);
                    foreach (var item in retorno.items)
                    {
                        Posicao posi = ObterPosicao(item);
                        if (posi != null)
                            posicoes.Add(posi);
                    }

                }

            }


            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }



        private RetornoItems ObterItems(string id, string token)
        {
            DateTime to_datetime = DateTime.Now;
            DateTime from_date = to_datetime.AddMinutes(-5);

            string url = $"{conta.Protocolo}://{conta.Servidor}/{conta.URI}/" + id + "/coordinates?version=2&from_datetime=" + from_date.ToString("yyyy-MM-ddTHH:mm:sszz") + "&to_datetime=" + to_datetime.ToString("yyyy-MM-ddTHH:mm:sszz") + "&api_key=" + token;
            string bodyResponseHandshake = RequestAsync(url).Result;

            RetornoItems retorno = Newtonsoft.Json.JsonConvert.DeserializeObject<RetornoItems>(bodyResponseHandshake);
            return retorno;
        }

        private List<ObjectsRetorno> ObterObjetos(string token)
        {

            string url = $"{conta.Protocolo}://{conta.Servidor}/{conta.URI}/" + "?version=1&api_key=" + token;
            string bodyResponseHandshake = RequestAsync(url).Result;

            List<ObjectsRetorno> objetosRetorno = JsonConvert.DeserializeObject<List<ObjectsRetorno>>(bodyResponseHandshake);

            return objetosRetorno;
        }

        private Posicao ObterPosicao(Items item)
        {
            Posicao p = null;

            try
            {

                p = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                {

                    Data = this.aplicarFuso ? Convert.ToDateTime(item.Datetime).AddHours(-3) : Convert.ToDateTime(item.Datetime),
                    DataCadastro = DateTime.Now,
                    DataVeiculo = this.aplicarFuso ? Convert.ToDateTime(item.Datetime).AddHours(-3) : Convert.ToDateTime(item.Datetime),
                    IDEquipamento = item.Object_id,
                    Placa = "",
                    Latitude = (double)item.Position.Latitude,
                    Longitude = (double)item.Position.Longitude,
                    Velocidade = item.Position.Speed,
                    Temperatura = 0,
                    SensorTemperatura = false,
                    Descricao = "",
                    NivelBateria = item.Inputs.Device_Inputs.Power_supply_voltage,
                    Ignicao = item.Ignition_status == "ON" ? 1 : 0,
                    Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.TrustTrack
                };

            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }

            return p;
        }



        private string ObterToken()
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

        private async Task<string> RequestAsync(string url)
        {
            var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrustTrack));
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var ret = await response.Content.ReadAsStringAsync();
            return ret;
        }


        #endregion
    }
}
