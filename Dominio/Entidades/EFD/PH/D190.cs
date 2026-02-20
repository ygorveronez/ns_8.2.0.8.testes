namespace Dominio.Entidades.EFD.PH
{
    public class D190 : RegistroPH
    {
        #region Construtores

        public D190()
            : base("D190")
        {
        }

        #endregion

        #region Propriedades

        public ConhecimentoDeTransporteEletronico CTe { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            string cst = this.CTe.CST == "91" ? "90" : this.CTe.CST;
            this.EscreverDado(string.IsNullOrEmpty(cst) ? 90 : int.Parse(cst), 3); //CST_ICMS
            this.EscreverDado(this.CTe.CFOP.CodigoCFOP, 4); //CFOP
            this.EscreverDado(this.CTe.AliquotaICMS); //Alíquota do ICMS
            this.EscreverDado(this.CTe.ValorAReceber); //VL_OPR
            this.EscreverDado(this.CTe.BaseCalculoICMS); //VL_BC_ICMS
            this.EscreverDado(this.CTe.ValorICMS); //VL_ICMS

            if (this.CTe.CST.Equals("20") || this.CTe.CST.Equals("70"))
                this.EscreverDado(this.CTe.BaseCalculoICMS * (this.CTe.PercentualReducaoBaseCalculoICMS / 100)); //VL_RED_BC
            else
                this.EscreverDado(0m); //VL_RED_BC

            this.EscreverDado(""); //COD_OBS

            this.FinalizarRegistro();

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
