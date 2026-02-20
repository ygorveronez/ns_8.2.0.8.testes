using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class PneuCustoEstoque
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Almoxarifado { get; set; }
        public PosicaoEixoPneu Posicao { get; set; }
        public int PosicaoNumero { get; set; }
        public string NumeroFogo { get; set; }
        public string Dimensao { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Banda { get; set; }
        public int KmRodado { get; set; }
        public decimal Sulco { get; set; }
        public decimal ValorAquisicao { get; set; }
        public decimal Custo { get; set; }
        public decimal CustoKM { get; set; }
        public VidaPneu VidaAtual { get; set; }
        public DateTime DataAquisicao { get; set; }
        public DateTime DataUltimaMovimentacao { get; set; }
        public string BandaRodagemUltimaMovimentacao { get; set; }
        public EstadoPneu EstadoAtualPneu { get; set; }
        public string Veiculo { get; set; }
        public string PlacaVeiculoDoEstepe { get; set; }
        public string Estepe { get; set; }
        public SituacaoPneu Situacao { get; set; }
        #endregion

        #region Propriedades com Regras

        public string PosicaoDescricao
        {
            get { return PosicaoNumero > 0 ? $"Eixo: {PosicaoNumero} - {Posicao.ObterDescricao()}" : Posicao.ObterDescricao(); }
        }

        public string VidaAtualDescricao
        {
            get { return VidaAtual.ObterDescricao(); }
        }

        public string DataAquisicaoFormatada
        {
            get { return DataAquisicao > DateTime.MinValue ? DataAquisicao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataUltimaMovimentacaoFormatada
        {
            get { return DataUltimaMovimentacao > DateTime.MinValue ? DataUltimaMovimentacao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string EstadoAtualPneuDescricao
        {
            get { return EstadoAtualPneu.ObterDescricao(); }
        }

        public string SituacaoDescricao
        {
            get { return Situacao.ObterDescricao(); }
        }
        #endregion
    }
}
