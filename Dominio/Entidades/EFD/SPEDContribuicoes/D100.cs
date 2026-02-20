namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class D100 : Registro
    {
        #region Construtores

        public D100()
            : base("D100")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.DocumentoEntrada CTe { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado("1"); //IND_OPER
            this.EscreverDado("0"); //IND_EMIT        
            this.EscreverDado(this.CTe.Fornecedor != null ? this.CTe.Fornecedor.CPF_CNPJ_SemFormato : ""); //COD_PART
            this.EscreverDado(this.CTe.Modelo.Numero); //COD_MOD
            this.EscreverDado("00"); //COD_SIT
            this.EscreverDado(this.CTe.Serie); //SER
            this.EscreverDado(""); //SUB
            this.EscreverDado(this.CTe.Numero.ToString()); //NUM_DOC
            this.EscreverDado(this.CTe.Chave); //CHV_CTE
            this.EscreverDado(this.CTe.DataEmissao); //DT_DOC
            this.EscreverDado(this.CTe.DataEmissao); //DT_A_P
            this.EscreverDado(Dominio.Enumeradores.TipoCTE.Normal.ToString("d")); //TP_CT-e
            this.EscreverDado(""); //CHV_CTE_REF
            this.EscreverDado(this.CTe.ValorTotal); //VL_DOC
            this.EscreverDado(this.CTe.ValorTotalDesconto); //VL_DESC
            this.EscreverDado("1"); //IND_FRT
            this.EscreverDado(this.CTe.ValorTotal); //VL_SERV
            this.EscreverDado(this.CTe.BaseCalculoICMS); //VL_BC_ICMS
            this.EscreverDado(this.CTe.ValorTotalICMS); //VL_ICMS
            this.EscreverDado(""); //VL_NT
            this.EscreverDado(""); //COD_INF
            this.EscreverDado(""); //COD_CTA

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }
        
        #endregion
    }
}
