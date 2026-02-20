namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class _1900 : Registro
    {
        #region Construtores

        public _1900() : base("1900") { }

        #endregion

        #region Propriedades

        public string CNPJ { get; set; }

        public string Modelo { get; set; }

        public string Serie { get; set; }

        public string SubSerie { get; set; }

        public string Situacao { get; set; }

        public decimal ValorTotalReceita { get; set; }

        public int QuantidadeTotalDocumentos { get; set; }

        public string CST_PIS { get; set; }

        public string CST_COFINS { get; set; }

        public string CFOP { get; set; }

        public string InformacoesComplementares { get; set; }

        public string ContaContabil { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CNPJ, 14); //CNPJ
            this.EscreverDado(this.Modelo, 2); //COD_MOD
            this.EscreverDado(this.Serie, 4); //SER
            this.EscreverDado(this.SubSerie, 20); //SUB_SER
            this.EscreverDado(this.Situacao, 2); //COD_SIT
            this.EscreverDado(this.ValorTotalReceita); //VL_TOT_REC
            this.EscreverDado(this.QuantidadeTotalDocumentos); //QUANT_DOC
            this.EscreverDado(this.CST_PIS, 2); //CST_PIS
            this.EscreverDado(this.CST_COFINS, 2); //CST_COFINS
            this.EscreverDado(this.CFOP, 4); //CFOP
            this.EscreverDado(this.InformacoesComplementares); //INF_COMPL
            this.EscreverDado(this.ContaContabil, 60); //COD_CTA

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
