namespace Dominio.Entidades.EFD.SPED
{
    public class C100 : Registro
    {
        #region Construtores

        public C100()
            : base("C100")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.DocumentoEntrada DocumentoEntrada { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado("0"); //IND_OPER
            this.EscreverDado("1"); //IND_EMIT
            this.EscreverDado(this.DocumentoEntrada.Fornecedor.CPF_CNPJ_SemFormato); //COD_PART
            this.EscreverDado(this.DocumentoEntrada.Modelo.Numero, 2); //COD_MOD
            this.EscreverDado("00"); //COD_SIT
            this.EscreverDado(this.DocumentoEntrada.Serie, 3); //SER
            this.EscreverDado(this.DocumentoEntrada.Numero.ToString(), 9); //NUM_DOC
            this.EscreverDado(this.DocumentoEntrada.Chave); //CHV_NFE
            this.EscreverDado(this.DocumentoEntrada.DataEmissao); //DT_DOC
            this.EscreverDado(this.DocumentoEntrada.DataEntrada); //DT_E_S
            this.EscreverDado(this.DocumentoEntrada.ValorTotal); //VL_DOC
            this.EscreverDado(this.DocumentoEntrada.IndicadorPagamento.ToString("D")); //IND_PGTO
            this.EscreverDado(this.DocumentoEntrada.ValorTotalDesconto); //VL_DESC
            this.EscreverDado(""); //VL_ABAT_NT
            this.EscreverDado(this.DocumentoEntrada.ValorProdutos + (this.DocumentoEntrada.BaseCalculoICMSST > 0 ? 0 : this.DocumentoEntrada.ValorTotalICMSST) ); //VL_MERC
            this.EscreverDado("0"); //IND_FRT
            this.EscreverDado(this.DocumentoEntrada.ValorTotalFrete); //VL_FRT
            this.EscreverDado(""); //VL_SEG
            this.EscreverDado(this.DocumentoEntrada.ValorTotalOutrasDespesas); //VL_OUT_DA
            this.EscreverDado(this.DocumentoEntrada.BaseCalculoICMS); //VL_BC_ICMS
            this.EscreverDado(this.DocumentoEntrada.ValorTotalICMS); //VL_ICMS
            this.EscreverDado(this.DocumentoEntrada.BaseCalculoICMSST); //VL_BC_ICMS_ST
            this.EscreverDado(this.DocumentoEntrada.BaseCalculoICMSST > 0 ? this.DocumentoEntrada.ValorTotalICMSST : 0); //VL_ICMS_ST
            this.EscreverDado(this.DocumentoEntrada.ValorTotalIPI); //VL_IPI
            this.EscreverDado(this.DocumentoEntrada.ValorTotalPIS); //VL_PIS
            this.EscreverDado(this.DocumentoEntrada.ValorTotalCOFINS); //VL_COFINS
            this.EscreverDado(""); //VL_PIS_ST
            this.EscreverDado(""); //VL_COFINS_ST

            this.FinalizarRegistro();
            this.ObterRegistrosSPEDDerivados();
            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
