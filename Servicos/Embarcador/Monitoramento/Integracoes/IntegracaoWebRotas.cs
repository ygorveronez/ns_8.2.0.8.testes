using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoWebRotas : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoWebRotas Instance;
        private static readonly string nameConfigSection = "WebRotas";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Construtor privado

        private IntegracaoWebRotas(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.WebRotas, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoWebRotas GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoWebRotas(cliente);
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
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.Positions> retornoRequisicao = BuscaUltimaPosicaoVeiculos();

                Log($"Recebidas posiçoes de {retornoRequisicao.Count} veículos", 3);

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.Positions p in retornoRequisicao)
                {
                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {

                        Data = p.Last_Position.Position_date_time,
                        DataCadastro = DateTime.Now,
                        DataVeiculo = p.Last_Position.Position_date_time,
                        IDEquipamento = p.Serial,
                        Placa = p.Vehicle.Plate.Replace("-","").Replace(" ",""),
                        Latitude = (double)p.Last_Position.Latitude,
                        Longitude = (double)p.Last_Position.Longitude,
                        Velocidade = p.Last_Position.Speed ?? 0,
                        Temperatura = p.Last_Position.Temperature,
                        SensorTemperatura = false,
                        Descricao = p.Serial,
                        NivelBateria = p.Last_Position.Voltage,
                        Ignicao = p.Last_Position.Position_ignition_state ?? 0,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.WebRotas
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

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.Positions> BuscaUltimaPosicaoVeiculos()
        {            
            Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.ResponseToken dadosToken = ObterToken();

            if (!string.IsNullOrEmpty(dadosToken.Access_token))
            {
                string url = $"{conta.Protocolo}://{conta.Servidor}/device/?includes=last_position,vehicle&map_filter=true&filter_sold=1";
                string bodyResponseHandshake = RequestAsync(url, dadosToken.Access_token).Result;
                //Json de retorno é tratado em seu primeiro nivel para facilitar o deserialize.
                bodyResponseHandshake = bodyResponseHandshake.Replace("{\"data\": ", "");
                bodyResponseHandshake = bodyResponseHandshake.Substring(0, bodyResponseHandshake.Length - 1);
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.Positions> posicoes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.Positions>>(bodyResponseHandshake);
                
                
                return posicoes;
            }
            else
            {
                return new List<Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.Positions>();
            }
        }


        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.Positions> TratarObjetoDinamico(dynamic dados)
        {
            var retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.Positions>();

            foreach(var da in dados.data)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.Positions pos = new Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.Positions();
                
                //var posicao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.Positions>(p);
                //retorno.Add(posicao);
            }

            return retorno;
        }


        private Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.ResponseToken ObterToken()
        {


            Log("Obtendo Token", 2);

            Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.RequestToken handshake = new Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.RequestToken();
            handshake.Username = this.conta.Usuario;
            handshake.Password = this.conta.Senha;

            string jsonRequestBody = JsonConvert.SerializeObject(handshake, Formatting.Indented);

            // Request
            string url = $"{conta.Protocolo}://{conta.Servidor}/{conta.URI}/";

            var bodyResponseHandshake = RequestAsync(url, null, jsonRequestBody).Result;

            Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.ResponseToken responseHandshake = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas.ResponseToken>(bodyResponseHandshake);

            return responseHandshake;
        }

        private async Task<string> RequestAsync(string url, string token = null, string body = null)
        {
            var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoWebRotas));
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            
            if (!string.IsNullOrEmpty(body))
            {
                var content = new StringContent(body.ToLower(), null, "application/json");
                request.Content = content;
            }   

            if (!string.IsNullOrEmpty(token))
            {
                request.Method = HttpMethod.Get;
                request.Headers.Add("Authorization", "Bearer " + token);
            }               
            
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var ret = await response.Content.ReadAsStringAsync();
            return ret;
        }

 
        #endregion
    }
}
