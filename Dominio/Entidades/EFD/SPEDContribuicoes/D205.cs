namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class D205: Registro
    {
        #region Construtores

        public D205() : base("D205") { }

        #endregion

        #region Propriedades

        public string CST_COFINS { get; set; }

        public decimal ValorTotalItens { get; set; }

        public decimal ValorBaseCalculoCOFINS { get; set; }

        public decimal AliquotaCOFINS { get; set; }

        public decimal ValorCOFINS { get; set; }

        public string ContaContabil { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CST_COFINS, 2); //CST_COFINS
            this.EscreverDado(this.ValorTotalItens); //VL_ITEM
            this.EscreverDado(this.ValorBaseCalculoCOFINS); //VL_BC_COFINS
            this.EscreverDado(this.AliquotaCOFINS); //ALIQ_COFINS
            this.EscreverDado(this.ValorCOFINS); //VL_COFINS
            this.EscreverDado(this.ContaContabil); //COD_CTA

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
