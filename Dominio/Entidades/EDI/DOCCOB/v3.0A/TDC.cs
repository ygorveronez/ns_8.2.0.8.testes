namespace Dominio.Entidades.EDI.DOCCOB.v30A
{
    public class TDC:Registro
    {
        #region Construtores

        public TDC(int quantidadeTotalCTes, decimal valorTotalCTes)
            : base("355")
        {
            this.QuantidadeTotalCTes = quantidadeTotalCTes;
            this.ValorTotalCTes = valorTotalCTes;
        }

        #endregion

        #region Propriedades

        private decimal ValorTotalCTes { get; set; }

        private int QuantidadeTotalCTes { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.QuantidadeTotalCTes, 4);     //2.		QTDE. TOTAL DOCTOS. DE COBRANÇA
            this.EscreverDado(this.ValorTotalCTes, 13, 2);      //3.		VALOR TOTAL DOCTOS. DE COBRANÇA
            this.EscreverDado(' ', 148);                        //4.		FILLER

            this.FinalizarRegistro();

            return this.StringRegistro.ToString();
        }

        #endregion
    }
}
