using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoRaster : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoRaster Instance;
        private static readonly string nameConfigSection = "Raster";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Construtor privado

        private IntegracaoRaster(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoRaster GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoRaster(cliente);
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
                List<PosicaoIntegracaoRaster> posicoesRecebidas = BuscaPosicoesVeiculo();
                foreach (PosicaoIntegracaoRaster ultimaPosicaoVeiculo in posicoesRecebidas)
                    posicoes.Add(ObtemPosicao(ultimaPosicaoVeiculo));

                Log($"Recebidas posiçoes de {posicoes.GroupBy(x => x.Placa).ToList().Count} veículos", 3);

                Log($"{posicoes.Count} posicoes", 3);
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
                Log("Erro ObterPosicoes " + ex.InnerException.Message, 3);
            }

            return posicoes;
        }

        private List<PosicaoIntegracaoRaster> BuscaPosicoesVeiculo()
        {
            List<PosicaoIntegracaoRaster> posicoesRecebidas = new List<PosicaoIntegracaoRaster>();

            dynamic positionRequest = new
            {
                Ambiente = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().AmbienteProducao ? "Producao" : "Homologacao",
                Login = this.conta.Usuario,
                Senha = this.conta.Senha,
                TipoRetorno = "JSON",
                TipoConsulta = "Ultimas",
                CodUltPosicao = 0
            };

            string jsonRequestBody = JsonConvert.SerializeObject(positionRequest, Formatting.Indented);

            ResponseIntegracaoRaster retorno = null;

            var client = new RestClient(Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta));
            var request = new RestRequest(@"""getPosicoes""", Method.POST);
            request.AddJsonBody(jsonRequestBody);

            IRestResponse response = client.Execute(request);
            if (!response.IsSuccessful) throw new Exception(JsonConvert.SerializeObject(response));

            retorno = JsonConvert.DeserializeObject<ResponseIntegracaoRaster>(response.Content);
            if (retorno != null)
            {
                ResultIntegracaoRaster result = retorno.Result[0];
                if (result.CodErro > 0) throw new Exception(result.MsgErro);

                posicoesRecebidas = result.Posicoes;
            }

            return posicoesRecebidas;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao ObtemPosicao(PosicaoIntegracaoRaster posicaoRaster)
        {
            DateTime dataVeiculo = DateTime.Now;
            try
            {
                dataVeiculo = Convert.ToDateTime(posicaoRaster.DataHoraPos ?? DateTime.Now);
            }
            catch (Exception e) 
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao converter data de posição Raster - continuando processamento: {e.ToString()}", "CatchNoAction");
            }; //Se não conseguiu converter a data, segue o baile.

            return new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
            {
                Data = dataVeiculo,
                DataCadastro = DateTime.Now,
                DataVeiculo = dataVeiculo,
                IDEquipamento = posicaoRaster.CodTerminal,
                Placa = posicaoRaster.Placa.Replace("-", ""),
                Latitude = posicaoRaster.Latitude,
                Longitude = posicaoRaster.Longitude,
                Velocidade = posicaoRaster.Velocidade ?? 0,
                Temperatura = 0,
                SensorTemperatura = false,
                Descricao = posicaoRaster.PosReferencia,
                NivelBateria = 0,
                Ignicao = posicaoRaster.Ignicao == "L" ? 1 : 0,
                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Raster
            };
        }


        #endregion
    }
    public class ResponseIntegracaoRaster
    {
        public List<ResultIntegracaoRaster> Result { get; set; }
    }
    public class ResultIntegracaoRaster
    {
        public string Ambiente { get; set; }
        public string Metodo { get; set; }
        public string Login { get; set; }
        public int CodErro { get; set; }
        public string MsgErro { get; set; }
        public List<PosicaoIntegracaoRaster> Posicoes { get; set; }
    }
    public class PosicaoIntegracaoRaster
    {
        public string CodPosicao { get; set; }
        public string Placa { get; set; }
        public string CodTerminal { get; set; }
        public string TipoRastreador { get; set; }
        public DateTime? DataHoraPos { get; set; }
        public long DistUltPosicao { get; set; }
        public string Ignicao { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PosReferencia { get; set; }
        public int? Velocidade { get; set; }
        public long VeloMediaCalc { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Pais { get; set; }
    }
}

