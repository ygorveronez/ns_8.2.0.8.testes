using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class ConsultaBiddingOferta
    {
        private int _rodada;

        public int Codigo { get; set; }
        public int SituacaoEnum { get; set; }
        public int Ranking { get; set; }
        public int RotaCodigo { get; set; }
        public string RotaDescricao { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Target { get; set; }
        public string Rodada { get; set; }
        public string Situacao { get; set; }
        public string MenorValor { get; set; }
        public string Baseline { get; set; }
        public string QuantidadeEntregas { get; set; }
        public string QuantidadeAjudantes { get; set; }
        public string QuantidadeViagensAno { get; set; }
        public string QuantidadeViagensAnoTransportador { get; set; }
        public int VolumeTonAno { get; set; }
        public int VolumeTonAnoTransportador { get; set; }
        public string Transportador { get; set; }
        public string ImpactoReais { get; set; }
        public string ImpactoPercentual { get; set; }
        public string Spend { get; set; }
        public decimal SpendPercentual { get; set; }
        public string RegiaoOrigem { get; set; }
        public string RegiaoDestino { get; set; }
        public string ValorFrete { get; set; }
        public string AliquotaICMS { get; set; }
        public string TransportadorValorFrete { get; set; }
        public string AdicionalPorEntrega { get; set; }
        public string TransportadorPedagio { get; set; }
        public string Pedagio { get; set; }
        public string TransportadorAdicionalPorEntrega { get; set; }
        public string Ajudante { get; set; }
        public string TransportadorAjudante { get; set; }
        public string Total { get; set; }
        public string SegundoMenorValor { get; set; }
        public string SegundoTransportador { get; set; }
        public string TerceiroMenorValor { get; set; }
        public string TerceiroTransportador { get; set; }
        public List<Baseline> Baselines { get; set; }
        public List<CelulaPersonalizada> CelulasPersonalizadas { get; set; }
        public List<ColunaTrasportador> ColunasTrasportador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLanceBidding tipoLanceBidding { get; set; }
        public string VeiculosVerdes { get; set; }

        public string BaselinesString
        {
            get
            {
                return JsonConvert.SerializeObject(Baselines);
            }
        }

        public string CelulasPersonalizadasString
        {
            get
            {
                return JsonConvert.SerializeObject(CelulasPersonalizadas);
            }
        }

        public string ColunasTrasportadorString
        {
            get
            {
                return JsonConvert.SerializeObject(ColunasTrasportador);
            }
            set
            {
                ColunasTrasportador = string.IsNullOrWhiteSpace(value)
                                ? new List<ColunaTrasportador>()
                                : JsonConvert.DeserializeObject<List<ColunaTrasportador>>(value);
            }
        }

        public void SetRodada(int rodada)
        {
            _rodada = rodada;
        }

        public bool IsUltimaRodadaMaiorQueUm(int ultimaRodada)
        {
            return _rodada > 1 && _rodada == ultimaRodada;
        }

        #region Colunas Dinamicas

        public string ColunaDinamicaBaseLine1 { get; set; }
        public string ColunaDinamicaBaseLine2 { get; set; }
        public string ColunaDinamicaBaseLine3 { get; set; }
        public string ColunaDinamicaBaseLine4 { get; set; }
        public string ColunaDinamicaBaseLine5 { get; set; }
        public string ColunaDinamicaBaseLine6 { get; set; }
        public string ColunaDinamicaBaseLine7 { get; set; }
        public string ColunaDinamicaBaseLine8 { get; set; }
        public string ColunaDinamicaBaseLine9 { get; set; }
        public string ColunaDinamicaBaseLine10 { get; set; }

        #endregion
    }
}
