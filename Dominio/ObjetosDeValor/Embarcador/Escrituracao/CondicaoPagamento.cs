namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public sealed class CondicaoPagamento
    {
        public int? CodigoTipoCarga { get; set; }

        public int? CodigoTipoOperacao { get; set; }

        public int? DiaEmissaoLimite { get; set; }

        public int? DiaMes { get; set; }

        public int? DiasDePrazoPagamento { get; set; }

        public Enumeradores.DiaSemana? DiaSemana { get; set; }

        public Enumeradores.TipoPrazoPagamento? TipoPrazoPagamento { get; set; }

        public bool VencimentoForaMes { get; set; }
        public bool? ConsiderarDiaUtilVencimento { get; set; }
    }
}
