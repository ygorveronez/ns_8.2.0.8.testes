namespace Dominio.Entidades.EFD.SPED
{
    public class C170 : Registro
    {
        #region Construtores

        public C170()
            : base("C170")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.ItemDocumentoEntrada Item { get; set; }

        public int versaoAtoCOTEPE { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Item.Sequencial); //NUM_ITEM
            if (this.Item.Produto.CodigoProduto != null && this.Item.Produto.CodigoProduto != "")
                this.EscreverDado(this.Item.Produto.CodigoProduto); //COD_ITEM
            else
                this.EscreverDado(this.Item.Produto.Codigo); //COD_ITEM
            this.EscreverDado(this.Item.Produto.Descricao); //DESCR_COMPL
            this.EscreverDado(this.Item.Quantidade); //QTD
            this.EscreverDado(this.Item.UnidadeMedida.Sigla, 6); //UNID
            this.EscreverDado(this.Item.ValorTotal); //VL_ITEM
            this.EscreverDado(this.Item.Desconto); //VL_DESC
            this.EscreverDado("1"); //IND_MOV
            this.EscreverDado(this.Item.CST, 3); //CST_ICMS
            this.EscreverDado(this.Item.CFOP.CodigoCFOP, 4); //CFOP
            this.EscreverDado(this.Item.CFOP.CodigoCFOP); //COD_NAT
            this.EscreverDado(this.Item.BaseCalculoICMS); //VL_BC_ICMS
            this.EscreverDado(this.Item.AliquotaICMS); //ALIQ_ICMS
            this.EscreverDado(this.Item.ValorICMS); //VL_ICMS
            this.EscreverDado(this.Item.BaseCalculoICMSST); //VL_BC_ICMS_ST
            this.EscreverDado(""); //ALIQ_ST
            this.EscreverDado(this.Item.BaseCalculoICMSST > 0 ? this.Item.ValorICMSST : 0); //VL_ICMS_ST
            this.EscreverDado(""); //IND_APUR
            this.EscreverDado(this.Item.CSTIPI); //CST_IPI
            this.EscreverDado(""); //COD_ENQ
            this.EscreverDado(this.Item.BaseCalculoIPI); //VL_BC_IPI
            this.EscreverDado(this.Item.AliquotaIPI); //ALIQ_IPI
            this.EscreverDado(this.Item.ValorIPI); //VL_IPI
            this.EscreverDado(""); //CST_PIS
            this.EscreverDado(""); //VL_BC_PIS
            this.EscreverDado(""); //ALIQ_PIS
            this.EscreverDado(""); //QUANT_BC_PIS
            this.EscreverDado(""); //ALIQ_PIS
            this.EscreverDado(""); //VL_PIS
            this.EscreverDado(""); //CST_COFINS
            this.EscreverDado(""); //VL_BC_COFINS
            this.EscreverDado(""); //ALIQ_COFINS
            this.EscreverDado(""); //QUANT_BC_COFINS
            this.EscreverDado(""); //ALIQ_COFINS
            this.EscreverDado(""); //VL_COFINS
            this.EscreverDado(""); //COD_CTA
            if (versaoAtoCOTEPE >= 13) //2019
                this.EscreverDado(""); //VL_ABAT_NT

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
