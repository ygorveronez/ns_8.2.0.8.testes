namespace Dominio.Entidades.EDI.CONEMB.v30A
{
    public class TCE : Registro
    {

        #region Construtores

        public TCE(int quantidadeTotalCTes, decimal valorTotalCTes)
            : base("323")
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
            this.EscreverDado(this.QuantidadeTotalCTes, 4); //2.		QUANTIDADE TOTAL DE CONHECIMENTOS
            this.EscreverDado(this.ValorTotalCTes, 13, 2); //3.		VALOR TOTAL DOS CONHECIMENTOS
            this.EscreverDado(' ', 658); //4.		FILLER

            this.FinalizarRegistro();

            return this.StringRegistro.ToString();
        }

        #endregion

    }
}
