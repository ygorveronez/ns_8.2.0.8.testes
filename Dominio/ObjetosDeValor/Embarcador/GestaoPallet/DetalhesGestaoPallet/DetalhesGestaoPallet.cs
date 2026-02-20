namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.DetalhesGestaoPallet
{
    public class DetalhesGestaoPallet
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string Situacao { get; set; }

        public Carga Carga { get; set; }

        public int QuantidadePallets { get; set; }

        public string Observacao { get; set; }

        public XMLNotaFiscal XMLNotaFiscal { get; set; }

        public int? NumeroNotaFiscalPermuta { get; set; }

        public string SerieNotaFiscalPermuta { get; set; }

        public int? NumeroNotaFiscalDevolucao { get; set; }

        public string SerieNotaFiscalDevolucao {  get; set; }

        #endregion Propriedades
    }
}