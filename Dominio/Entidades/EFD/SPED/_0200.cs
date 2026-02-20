namespace Dominio.Entidades.EFD.SPED
{
    public class _0200 : Registro
    {
        #region Construtores

        public _0200()
            : base("0200")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.Produto Produto { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            if (this.Produto.CodigoProduto != null && this.Produto.CodigoProduto != "")
                this.EscreverDado(this.Produto.CodigoProduto, 60); //COD_ITEM
            else
                this.EscreverDado(this.Produto.Codigo.ToString(), 60); //COD_ITEM
            this.EscreverDado(this.Produto.Descricao); //DESCR_ITEM
            this.EscreverDado(""); //COD_BARRA
            this.EscreverDado(""); //COD_ANTERIOR_ITEM
            this.EscreverDado(this.Produto.UnidadeMedida.Sigla); //UNID_INV
            this.EscreverDado("00"); //TIPO_ITEM
            this.EscreverDado(this.Produto.NCM.Numero, 8); //COD_NCM
            this.EscreverDado(""); //EX_IPI
            this.EscreverDado(""); //COD_GEN
            this.EscreverDado(""); //COD_LST
            this.EscreverDado(0m); //ALIQ_ICMS
            this.EscreverDado(""); //CEST

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
