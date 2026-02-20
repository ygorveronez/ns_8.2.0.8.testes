namespace Dominio.Entidades.EFD.PH
{
    public class _1010 : RegistroPH
    {
        #region Construtores

        public _1010()
            : base("1010")
        {
        }

        #endregion

        #region Propriedades

        public int versaoAtoCOTEPE { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado("N"); //IND_EXP
            this.EscreverDado("N"); //IND_CCRF
            this.EscreverDado("N"); //IND_COMB
            this.EscreverDado("N"); //IND_USINA
            this.EscreverDado("N"); //IND_VA
            this.EscreverDado("N"); //IND_EE
            this.EscreverDado("N"); //IND_CART
            this.EscreverDado("N"); //IND_FORM
            this.EscreverDado("N"); //IND_AER
            if (versaoAtoCOTEPE >= 13) //2019
            {
                this.EscreverDado("N"); //IND_GIAF1
                this.EscreverDado("N"); //IND_GIAF3
                this.EscreverDado("N"); //IND_GIAF4
            }
            if (versaoAtoCOTEPE >= 14) //2020
                this.EscreverDado("N");
            this.FinalizarRegistro();

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
