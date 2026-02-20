namespace Dominio.Entidades.EFD.SPED
{
    public class _0150 : Registro
    {
        #region Construtores

        public _0150()
            : base("0150")
        {
        }

        #endregion

        #region Propriedades

        public Cliente Cliente { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Cliente.CPF_CNPJ_SemFormato); //COD_PART
            this.EscreverDado(this.Cliente.Nome, 100); //NOME
            this.EscreverDado(this.Cliente.Localidade.Estado.Pais.Sigla); //COD_PAIS
            this.EscreverDado(this.Cliente.Tipo.Equals("J") ? this.Cliente.CPF_CNPJ_SemFormato : ""); //CNPJ
            this.EscreverDado(this.Cliente.Tipo.Equals("F") ? this.Cliente.CPF_CNPJ_SemFormato : ""); //CPF
            this.EscreverDado(Utilidades.String.OnlyNumbers(this.Cliente.IE_RG)); //IE
            this.EscreverDado(this.Cliente.Localidade.CodigoIBGE); //COD_MUN
            this.EscreverDado(this.Cliente.InscricaoSuframa); //SUFRAMA
            this.EscreverDado(this.Cliente.Endereco, 60); //END
            this.EscreverDado(this.Cliente.Numero, 10); //NUM
            this.EscreverDado(this.Cliente.Complemento, 60); //COMPL
            this.EscreverDado(this.Cliente.Bairro, 60); //BAIRRO

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
