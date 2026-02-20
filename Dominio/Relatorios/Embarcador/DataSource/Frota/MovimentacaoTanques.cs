using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class MovimentacaoTanques
    {
        public string LocalArmazenamento { get; set; }
        public int CodigoLocalArmazenamento { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime? DataEntradaMovimentacaoDetalhes { get; set; }
        public DateTime? DataSaidaMovimentacaoDetalhes { get; set; }
        public DateTime DataExibirDetalhes { get; set; }
        public string DiaMesAnoDataExibirDetalhes { get; set; }
        public string HorasMinutosDataExibirDetalhes { get; set; }
        public decimal SaldoInicialTanque { get; set; }
        public decimal ValorEntradaMovimentacao { get; set; }
        public decimal ValorSaidaMovimentacao { get; set; }
        public decimal ValorSaidaMovimentacaoDetalhes { get; set; }
        public decimal ValorEntradaMovimentacaoDetalhes { get; set; }
        public decimal? SaldoAtualTanque { get; set; }
    }
}
