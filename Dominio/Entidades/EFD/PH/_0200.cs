using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.EFD.PH
{
    public class _0200 : RegistroPH
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
            this.EscreverDado(this.Produto.CodigoBarrasEAN); //COD_BARRA
            this.EscreverDado(""); //COD_ANTERIOR_ITEM
            this.EscreverDado(UnidadeDeMedidaHelper.ObterSigla(this.Produto.UnidadeDeMedida)); //UNID_INV
            this.EscreverDado(CategoriaProdutoHelper.ObterSigla(this.Produto.CategoriaProduto), 2); //TIPO_ITEM
            this.EscreverDado(this.Produto.NCM.Codigo, 8); //COD_NCM
            this.EscreverDado(this.Produto.CodigoEnquadramentoIPI, 3); //EX_IPI
            this.EscreverDado(GeneroProdutoHelper.ObterSigla(this.Produto.GeneroProduto), 2); //COD_GEN
            this.EscreverDado(""); //COD_LST
            this.EscreverDado(0m); //ALIQ_ICMS
            this.EscreverDado(this.Produto.CEST.Numero);//CEST
            this.EscreverDado("_SPED_PH_");//_SPED_PH_"
            this.EscreverDado(0);//COD_TAB_REFR_FB
            this.EscreverDado("");//COD_GRUPO
            this.EscreverDado("");//MARC_REG
            this.EscreverDado(0);//TAB_PIS_COFINS
            this.EscreverDado("");//ITE_TAB_PIS_COFINS
            this.EscreverDado("");//SUB_ITEM_PIS_COFINS
            this.EscreverDado(0m); //FAT_CONV_QTDE_PIS_COFINS
            this.EscreverDado("0");//APU_SEC_PIS_COFINS
            this.EscreverDado(0m); //VL_BC_ICMS_ST
            this.EscreverDado(0m); //VL_RED_BC
            this.EscreverDado("0");//CRED_PRES
            this.EscreverDado("");//CFOP_ECF
            this.EscreverDado(0m); //ALIQ_IPI
            this.EscreverDado(0m); //ALIQ_ISS
            this.EscreverDado("");//COD_ATIV_SERV
            this.EscreverDado("");//COD_SERV
            this.EscreverDado("");//COD_DNF
            this.EscreverDado(0m); //FAT_CONV_UE
            this.EscreverDado("");//CAP_VOLUM

            this.FinalizarRegistro();

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
