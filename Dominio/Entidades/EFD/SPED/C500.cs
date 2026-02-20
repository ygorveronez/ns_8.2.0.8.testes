namespace Dominio.Entidades.EFD.SPED
{
    public class C500: Registro
    {
        #region Construtores

        public C500()
            : base("C500")
        {

        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.DocumentoEntrada DocumentoEntrada { get; set; }

        public int versaoAtoCOTEPE { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado("0"); //IND_OPER
            this.EscreverDado("1"); //IND_EMIT
            this.EscreverDado(this.DocumentoEntrada.Fornecedor.CPF_CNPJ_SemFormato); //COD_PART
            this.EscreverDado(this.DocumentoEntrada.Modelo.Numero, 2); //COD_MOD
            this.EscreverDado("00"); //COD_SIT
            this.EscreverDado(this.DocumentoEntrada.Serie, 4); //SER
            this.EscreverDado(""); //SUB
            this.EscreverDado("02"); //COD_CONS
            this.EscreverDado(this.DocumentoEntrada.Numero); //NUM_DOC
            this.EscreverDado(this.DocumentoEntrada.DataEmissao); //DT_DOC
            this.EscreverDado(this.DocumentoEntrada.DataEntrada); //DT_E_S
            this.EscreverDado(this.DocumentoEntrada.ValorTotal); //VL_DOC
            this.EscreverDado(""); //VL_DESC
            this.EscreverDado(this.DocumentoEntrada.ValorTotal); //VL_FORN
            this.EscreverDado(""); //VL_SERV_NT
            this.EscreverDado(""); //VL_TERC
            this.EscreverDado(""); //VL_DA
            this.EscreverDado(this.DocumentoEntrada.BaseCalculoICMS); //VL_BC_ICMS
            this.EscreverDado(this.DocumentoEntrada.ValorTotalICMS); //VL_ICMS
            this.EscreverDado(""); //VL_BC_ICMS_ST
            this.EscreverDado(""); //VL_ICMS_ST
            this.EscreverDado(""); //COD_INF
            this.EscreverDado(this.DocumentoEntrada.ValorTotalPIS); //VL_PIS
            this.EscreverDado(this.DocumentoEntrada.ValorTotalCOFINS); //VL_COFINS
            this.EscreverDado(""); //TP_LIGACAO
            this.EscreverDado(""); //COD_GRUPO_TENSAO

            if (versaoAtoCOTEPE >= 14)
            {
                this.EscreverDado(""); //CHV_DOCe --Chave da Nota Fiscal de Energia Elétrica Eletrônica
                this.EscreverDado("1"); //FIN_DOCe
                this.EscreverDado(""); //CHV_DOCe_REF
                this.EscreverDado("1"); //IND_DEST
                this.EscreverDado(this.DocumentoEntrada.Empresa.Localidade.CodigoIBGE.ToString()); //COD_MUN_DEST
                this.EscreverDado(""); //COD_CTA
                this.EscreverDado(""); //COD_MOD_DOC_REF
                this.EscreverDado(""); //HASH_DOC_REF
                this.EscreverDado(""); //SER_DOC_REF
                this.EscreverDado(""); //NUM_DOC_REF
                this.EscreverDado(""); //MES_DOC_REF
                this.EscreverDado(""); //ENER_INJET
                this.EscreverDado(""); //OUTRAS_DED 
            }

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }
        
        #endregion
    }
}
