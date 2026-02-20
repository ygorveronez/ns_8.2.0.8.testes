namespace Dominio.Entidades.SINTEGRA
{
    public class _71 : Registro
    {
        #region Construtores

        public _71()
            : base("71")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.DocumentosCTE Documento { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(!string.IsNullOrWhiteSpace(this.Documento.CTE.Tomador.CPF_CNPJ_SemFormato) ? long.Parse(this.Documento.CTE.Tomador.CPF_CNPJ_SemFormato) : 0, 14); //CNPJ do tomador
            this.EscreverDado(this.Documento.CTE.Tomador.IE_RG, 14); //Inscrição Estadual do tomador
            this.EscreverDado(this.Documento.CTE.DataEmissao); //Data de emissão
            this.EscreverDado(this.Documento.CTE.Tomador.Localidade?.Estado?.Sigla ?? string.Empty, 2); //Unidade da Federação do tomador
            this.EscreverDado(this.Documento.CTE.ModeloDocumentoFiscal.Numero, 2); //Modelo
            this.EscreverDado(this.Documento.CTE.Serie.Numero.ToString(), 1); //Série
            this.EscreverDado("", 2); //Subsérie
            this.EscreverDado(this.Documento.CTE.Numero, 6); //Número
            this.EscreverDado(this.Documento.CTE.Remetente.Localidade.Estado.Sigla, 2); //Unidade da Federação do remetente/ destinatário da nota fiscal
            this.EscreverDado(this.Documento.CTE.Remetente.CPF_CNPJ_SemFormato, 14); //CNPJ do remetente/destinatário da nota fiscal
            this.EscreverDado(this.Documento.CTE.Remetente.IE_RG, 14); //Inscrição Estadual do remetente/ destinatário da nota fiscal
            this.EscreverDado(this.Documento.DataEmissao); //Data de emissão da Nota fiscal
            this.EscreverDado(this.Documento.ModeloDocumentoFiscal != null ? this.Documento.ModeloDocumentoFiscal.Numero : !string.IsNullOrWhiteSpace(this.Documento.ChaveNFE) ? "57" : "01", 2); //Modelo da nota fiscal
            this.EscreverDado(this.Documento.SerieOuSerieDaChave, 3); //Série da nota fiscal

            int numeroNota = 0;

            if (int.TryParse(this.Documento.Numero, out numeroNota))
                this.EscreverDado(numeroNota, 6); //Número da nota fiscal
            else
                this.EscreverDado(this.Documento.Numero, 6);

            this.EscreverDado(this.Documento.Valor, 12, 2); //Valor total da nota fiscal
            this.EscreverDado("", 12); //Brancos

            this.FinalizarRegistro();

            this.ObterRegistrosSINTEGRADerivados();

            return this.RegistroSINTEGRA.ToString();
        }

        #endregion
    }
}
