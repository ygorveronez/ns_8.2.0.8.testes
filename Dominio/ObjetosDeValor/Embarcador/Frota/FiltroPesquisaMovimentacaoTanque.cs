using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaMovimentacaoTanques
    {
        public int? LocalArmazenamento { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public decimal SaldoInicialTanque { get; set; }
        public decimal ValorEntradaMovimentacao { get; set; }
        public decimal ValorSaidaMovimentacao { get; set; }
        public decimal SaldoAtualTanque { get; set; }
        public bool PossuiTipoOleoIdentificado { get; set; }
    }
}
