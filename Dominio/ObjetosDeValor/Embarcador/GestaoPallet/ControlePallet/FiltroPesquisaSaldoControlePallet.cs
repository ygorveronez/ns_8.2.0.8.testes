namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet
{
    public sealed class FiltroPesquisaSaldoControlePallet
    {
        public int Filial { get; set; }

        public int Transportador { get; set; }

        public long Cliente { get; set; }

        public Enumeradores.ResponsavelPallet? ResponsavelMovimentacaoPallet { get; set; }
        public Enumeradores.RegraPallet? RegraPallet { get; set; }

    }
}
