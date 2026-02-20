namespace Dominio.Entidades.EFD.PH
{
    public class D160 : RegistroPH
    {

        #region Construtores

        public D160()
            : base("D160")
        {
        }

        #endregion

        #region Propriedades

        public ConhecimentoDeTransporteEletronico CTe { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(""); //DESPACHO
            this.EscreverDado(this.CTe.Remetente.CPF_CNPJ_SemFormato); //CNPJ_CPF_REM
            this.EscreverDado(this.CTe.Remetente.IE_RG); //IE_REM
            this.EscreverDado(this.CTe.LocalidadeInicioPrestacao.Estado.Sigla == "EX" ? 9999999 : this.CTe.LocalidadeInicioPrestacao.CodigoIBGE, 7); //COD_MUN_ORI
            this.EscreverDado(this.CTe.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty); //CNPJ_CPF_DEST
            this.EscreverDado(this.CTe.Destinatario?.IE_RG ?? string.Empty); //IE_DEST
            this.EscreverDado(this.CTe.Destinatario != null ? this.CTe.Destinatario.Exterior ? 9999999 : this.CTe.Destinatario.Localidade.CodigoIBGE : 0000000); //COD_MUN_DEST
            this.EscreverDado("_SPED_PH_"); //IDE_PH
            this.EscreverDado(this.CTe.TomadorPagador?.Localidade.Estado.Sigla ?? string.Empty); //UF

            this.FinalizarRegistro();

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion

    }
}
