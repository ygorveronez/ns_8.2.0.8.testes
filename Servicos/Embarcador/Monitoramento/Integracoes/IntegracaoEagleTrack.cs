using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Infrastructure.Services.HttpClientFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoEagleTrack : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoEagleTrack Instance;
        private static readonly string nameConfigSection = "EagleTrack";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Construtor privado

        private IntegracaoEagleTrack(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EagleTrack, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoEagleTrack GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoEagleTrack(cliente);
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
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.EagleTrack.Positions> retornoRequisicao = BuscaPosicoesVeiculo();
                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.EagleTrack.Positions p in retornoRequisicao)
                {
                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {
                        Data = p.data.ToDateTime(),
                        DataCadastro = DateTime.Now,
                        DataVeiculo = p.data.ToDateTime(),
                        Placa = p.placa,
                        Latitude = p.latitude ?? 0,
                        Longitude = p.longitude ?? 0,
                        Velocidade = p.velocidade ?? 0,
                        Temperatura = 0,
                        SensorTemperatura = false,
                        Descricao = p.endereco + " " + p.cidade,
                        Ignicao = (!p.ignicao ?? false) ? 0 : 1,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.EagleTrack,
                        IDEquipamento = ""
                    });
                }
                
                Log($"Recebidas posiçoes de {posicoes.GroupBy(x => x.Placa).ToList().Count} veículos", 3);

                Log($"{posicoes.Count} posicoes", 3);
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.EagleTrack.Positions> BuscaPosicoesVeiculo()
        {

            string url = $"{conta.Protocolo}://{conta.Servidor + conta.URI}";
            string bodyResponseHandshake = RequestAsync(url).Result;
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.EagleTrack.Positions> posicoes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.EagleTrack.Positions>>(bodyResponseHandshake);
            return posicoes;
        }

        private string ObterToken()
        {
            return Servicos.Embarcador.Logistica.ContaIntegracao.ObterBasicToken(this.conta);
        }

        private async Task<string> RequestAsync(string url)
        {
            var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoEagleTrack));

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", "Basic " + ObterToken());
            
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var ret = await response.Content.ReadAsStringAsync();
            return ret;
        }

 
        #endregion
    }
}
