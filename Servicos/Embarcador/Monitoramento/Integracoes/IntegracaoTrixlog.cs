using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoTrixlog : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoTrixlog Instance;
        private static readonly string nameConfigSection = "Trixlog";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Construtor privado

        private IntegracaoTrixlog(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trixlog, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoTrixlog GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoTrixlog(cliente);
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

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog.Positions> retornoRequisicao = BuscaUltimaPosicaoVeiculos();

                Log($"Recebidas posiçoes de {retornoRequisicao.Count} veículos", 3);

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog.Positions p in retornoRequisicao)
                {
                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {

                        Data = p.ReceptionDate.AddHours(-3),
                        DataCadastro = DateTime.Now,
                        DataVeiculo = p.Date.AddHours(-3),
                        IDEquipamento = p.moduloID.ToString(),
                        Placa = p.equipmentGpsUnitId,
                        Latitude = (double)p.Position.Latitude,
                        Longitude = (double)p.Position.Longitude,
                        Velocidade = p.Speed,
                        Temperatura = 0,
                        SensorTemperatura = false,
                        Descricao = "",
                        NivelBateria = 0,
                        Ignicao = p.EngneOn ? 1 : 0,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Trixlog
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

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog.Positions> BuscaUltimaPosicaoVeiculos()
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog.ResponseToken RespToken = ObterToken();

            if (!string.IsNullOrEmpty(RespToken.Token))
            {
                string url = $"{conta.Protocolo}://{conta.Servidor}/{conta.URI}";
                string bodyResponseHandshake = RequestAsync(url, RespToken.Token).Result;
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog.Positions> posicoes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog.Positions>>(bodyResponseHandshake);
                return posicoes;
            }
            else
            {
                return new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog.Positions>();
            }
        }


        private Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog.ResponseToken ObterToken()
        {
            Log("Obtendo Token", 2);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog.RequestToken handshake = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog.RequestToken();
            handshake.Login = this.conta.Usuario;
            handshake.Password = this.conta.Senha;

            string jsonRequestBody = JsonConvert.SerializeObject(handshake, Formatting.Indented);

            // Request
            string url = "https://dev.integrator.trixlog.com/auth";

            var bodyResponseHandshake = RequestAsync(url, null, jsonRequestBody).Result;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog.ResponseToken responseHandshake = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog.ResponseToken>(bodyResponseHandshake);

            return responseHandshake;
        }

        private async Task<string> RequestAsync(string url, string token = null, string body = null)
        {
            var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrixlog));
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            if (!string.IsNullOrEmpty(body))
            {
                var content = new StringContent(body, null, "application/json");
                request.Content = content;
            }

            if (!string.IsNullOrEmpty(token))
            {
                request.Method = HttpMethod.Get;
                request.Headers.Add("Authorization", token);
            }

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var ret = await response.Content.ReadAsStringAsync();
            return ret;
        }


        #endregion
    }
}
