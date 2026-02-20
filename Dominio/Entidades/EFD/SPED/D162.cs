namespace Dominio.Entidades.EFD.SPED
{
    public class D162 : Registro
    {

        #region Construtores

        public D162()
            : base("D162")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.DocumentosCTE Documento { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Documento.ModeloDocumentoFiscal.Numero); //COD_MOD                        
            this.EscreverDado(this.Documento.SerieOuSerieDaChave); //SER
            this.EscreverDado(this.Documento.Numero); //NUM_DOC
            this.EscreverDado(this.Documento.DataEmissao); //DT_DOC
            this.EscreverDado(this.Documento.Valor); //VL_DOC
            this.EscreverDado(this.Documento.ValorProdutos > 0m ? this.Documento.ValorProdutos : this.Documento.Valor); //VL_MERC
            this.EscreverDado(1); //QTD_VOL
            this.EscreverDado(this.Documento.Peso); //PESO_BRT
            this.EscreverDado(this.Documento.Peso); //PESO_LIQ
            
            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion

    }
}
