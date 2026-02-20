namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class F500 : Registro
    {
        #region Construtores

        public F500() : base("F500") { }

        #endregion

        #region Propriedades

        public decimal ValorTotalReceitaRecebida { get; set; }

        public string CST_PIS { get; set; }

        public decimal ValorDescontoPIS { get; set; }

        public decimal ValorBaseCalculoPIS { get; set; }

        public decimal AliquotaPIS { get; set; }

        public decimal ValorPIS { get; set; }

        public string CST_COFINS { get; set; }

        public decimal ValorDescontoCOFINS { get; set; }

        public decimal ValorBaseCalculoCOFINS { get; set; }

        public decimal AliquotaCOFINS { get; set; }

        public decimal ValorCOFINS { get; set; }

        public string Modelo { get; set; }

        public int CFOP { get; set; }

        public string ContaContabil { get; set; }

        public string InformacaoComplementar { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.ValorTotalReceitaRecebida); //VL_REC_CAIXA
            this.EscreverDado(this.CST_PIS, 2); //CST_PIS
            this.EscreverDado(this.ValorDescontoPIS); //VL_DESC_PIS
            this.EscreverDado(this.ValorBaseCalculoPIS); //VL_BC_PIS
            this.EscreverDado(this.AliquotaPIS); //ALIQ_PIS
            this.EscreverDado(this.ValorPIS); //VL_PIS
            this.EscreverDado(this.CST_COFINS, 2); //CST_COFINS
            this.EscreverDado(this.ValorDescontoCOFINS); //VL_DESC_COFINS
            this.EscreverDado(this.ValorBaseCalculoCOFINS); //VL_BC_COFINS
            this.EscreverDado(this.AliquotaCOFINS); //ALIQ_COFINS
            this.EscreverDado(this.ValorCOFINS); //VL_COFINS
            this.EscreverDado(this.Modelo); //COD_MOD
            this.EscreverDado(this.CFOP, 4); //CFOP
            this.EscreverDado(this.ContaContabil, 60); //COD_CTA
            this.EscreverDado(this.InformacaoComplementar); //INFO_COMPL

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
