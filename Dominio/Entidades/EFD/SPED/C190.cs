namespace Dominio.Entidades.EFD.SPED
{
    public class C190 : Registro
    {
        #region Construtores

        public C190()
            : base("C190")
        {
        }

        #endregion

        #region Propriedades

        public string CSTICMS { get; set; }

        public int CFOP { get; set; }

        public decimal AliquotaICMS { get; set; }

        public decimal ValorOperacao { get; set; }

        public decimal BaseCalculoICMS { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal BaseCalculoICMSST { get; set; }

        public decimal ValorICMSST { get; set; }

        public decimal ValorReducaoBaseCalculo { get; set; }

        public decimal ValorIPI { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CSTICMS, 3); //CST_ICMS
            this.EscreverDado(this.CFOP, 4); //CFOP
            this.EscreverDado(this.AliquotaICMS); //ALIQ_ICMS
            this.EscreverDado(this.ValorOperacao); //VL_OPR
            this.EscreverDado(this.BaseCalculoICMS); //VL_BC_ICMS
            this.EscreverDado(this.ValorICMS); //VL_ICMS
            this.EscreverDado(this.BaseCalculoICMSST); //VL_BC_ICMS_ST
            this.EscreverDado(this.ValorICMSST); //VL_ICMS_ST
            this.EscreverDado(this.ValorReducaoBaseCalculo); //VL_RED_BC
            this.EscreverDado(this.ValorIPI); //VL_IPI
            this.EscreverDado(""); //COD_OBS

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
