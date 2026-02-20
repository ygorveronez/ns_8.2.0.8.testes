namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class F525 : Registro
    {
        #region Construtores

        public F525() : base("F525") { }

        #endregion

        #region Propriedades

        public decimal ValorTotalReceitaRecebida { get; set; }

        public string IndicadorComposicao { get; set; }

        public string CPFCNPJParticipante { get; set; }

        public string NumeroDocumento { get; set; }

        public string CodigoItem { get; set; }

        public decimal ValorReceitaDetalhada { get; set; }

        public string CST_PIS { get; set; }

        public string CST_COFINS { get; set; }

        public string InformacaoComplementar { get; set; }

        public string ContaContabil { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.ValorTotalReceitaRecebida); //VL_REC
            this.EscreverDado(this.IndicadorComposicao, 2); //IND_REC
            this.EscreverDado(this.CPFCNPJParticipante, 14); //CNPJ_CPF
            this.EscreverDado(this.NumeroDocumento, 60); //NUM_DOC
            this.EscreverDado(this.CodigoItem, 60); //COD_ITEM
            this.EscreverDado(this.ValorReceitaDetalhada); //VL_REC_DET
            this.EscreverDado(this.CST_PIS, 2); //CST_PIS
            this.EscreverDado(this.CST_COFINS, 2); //CST_COFINS
            this.EscreverDado(this.InformacaoComplementar); //INFO_COMPL
            this.EscreverDado(this.ContaContabil, 60); //COD_CTA

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
