namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet
{
    public class TotalizadoresControleSaldoPallet
    {
        #region Propriedades

        public int TotalPendente { get; set; }

        public int TotalDevolvido { get; set; }

        public int PalettsPendente { get; set; }

        public int PalettsReservado { get; set; }

        public int TotalSaldo { get; set; }

        public int NotasNoPrazo { get; set; }

        public int NotasVencido { get; set; }

        public int NotasAgendado { get; set; }

        public int NotasPermuta { get; set; }

        public int PalettsNoPrazo { get; set; }

        public int PalettsVencido { get; set; }

        public int PalettsAgendado { get; set; }

        public int PalettsPermuta { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public int Total
        {
            get { return TotalPendente + TotalDevolvido; }
        }

        #endregion Propriedades com Regras
    }
}
