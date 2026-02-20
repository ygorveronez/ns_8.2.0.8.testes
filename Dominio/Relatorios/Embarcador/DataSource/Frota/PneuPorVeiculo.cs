using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public sealed class PneuPorVeiculo
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Veiculo { get; set; }
        public string MarcaPneu { get; set; }
        public string ModeloPneu { get; set; }
        public string CentroResultado { get; set; }
        private PosicaoEixoPneu Posicao { get; set; }
        public string NumeroFogo { get; set; }
        public string Dimensao { get; set; }
        private TipoBandaRodagemPneu TipoBandaRodagem { get; set; }
        public string BandaRodagem { get; set; }
        public string Segmento { get; set; }
        public int KMRodado { get; set; }
        public decimal Sulco { get; set; }
        public decimal ValorAquisicao { get; set; }
        public decimal ValorCusto { get; set; }
        public decimal ValorCustoPorKM { get; set; }
        private VidaPneu VidaAtual { get; set; }
        public string ModeloVeicular { get; set; }
        private int Numero { get; set; }
        public int Calibragem { get; set; }
        public decimal Milimitragem1 { get; set; }
        public decimal Milimitragem2 { get; set; }
        public decimal Milimitragem3 { get; set; }
        public decimal Milimitragem4 { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public DateTime DataMovimentacaoPneu { get; set; }

        #endregion

        public string DescricaoBandaRodagem
        {
            get { return TipoBandaRodagem.ObterDescricao(); }
        }

        public string DescricaoPosicao
        {
            get { return Posicao.ObterDescricao(); }
        }

        public string DescricaoVidaAtual 
        {
            get { return VidaAtual.ObterDescricao(); }
        }
        public string DescricaoNumero
        {
            get { return $"Eixo {Numero}"; }
        }
    }
}
