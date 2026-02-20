namespace Dominio.Entidades.EFD.PH
{
    public class D500 : RegistroPH
    {
        #region Construtores

        public D500()
            : base("D500")
        {
        }

        #endregion

        #region Propriedades

        public Embarcador.Financeiro.DocumentoEntradaTMS Documento { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado("0"); //IND_OPER
            this.EscreverDado("1"); //IND_EMIT
            this.EscreverDado(this.Documento.Fornecedor.CPF_CNPJ_SemFormato); //COD_PART
            this.EscreverDado(this.Documento.Modelo.Numero, 2); //COD_MOD
            this.EscreverDado("00"); //COD_SIT
            this.EscreverDado(this.Documento.Serie, 4); //SER
            this.EscreverDado(""); //SUB
            this.EscreverDado(this.Documento.Numero, 9); //NUM_DOC
            this.EscreverDado(this.Documento.DataEmissao); //DT_DOC
            this.EscreverDado(this.Documento.DataEntrada); //DT_A_P
            this.EscreverDado(this.Documento.ValorTotal); //VL_DOC
            this.EscreverDado(0m); //VL_DESC
            this.EscreverDado(this.Documento.ValorTotal); //VL_SERV
            this.EscreverDado(this.Documento.ValorTotal - this.Documento.BaseCalculoICMS); //VL_SERV_NT
            this.EscreverDado(0m); //VL_TERC
            this.EscreverDado(0m); //VL_DA
            this.EscreverDado(this.Documento.BaseCalculoICMS); //VL_BC_ICMS
            this.EscreverDado(this.Documento.ValorTotalICMS); //VL_ICMS
            this.EscreverDado(""); //COD_INF
            this.EscreverDado(this.Documento.ValorTotalPIS); //VL_PIS
            this.EscreverDado(this.Documento.ValorTotalCOFINS); //VL_COFINS
            //this.EscreverDado(this.Documento.PlanoDeConta.ContaContabil); //COD_CTA
            this.EscreverDado(""); //COD_CTA
            this.EscreverDado("1"); //TP_ASSINANTE

            this.FinalizarRegistro();

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
