namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class D201 : Registro
    {
        #region Construtores

        public D201() : base("D201") { }

        #endregion

        #region Propriedades

        public string CST_PIS { get; set; }

        public decimal ValorTotalItens { get; set; }

        public decimal ValorBaseCalculoPIS { get; set; }

        public decimal AliquotaPIS { get; set; }

        public decimal ValorPIS { get; set; }

        public string ContaContabil { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CST_PIS, 2); //CST_PIS
            this.EscreverDado(this.ValorTotalItens); //VL_ITEM
            this.EscreverDado(this.ValorBaseCalculoPIS); //VL_BC_PIS
            this.EscreverDado(this.AliquotaPIS); //ALIQ_PIS
            this.EscreverDado(this.ValorPIS); //VL_PIS
            this.EscreverDado(this.ContaContabil); //COD_CTA

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
