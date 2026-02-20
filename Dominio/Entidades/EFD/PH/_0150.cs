namespace Dominio.Entidades.EFD.PH
{
    public class _0150 : RegistroPH
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
            this.EscreverDado("_SPED_PH_", 9); //_SPED_PH_"
            this.EscreverDado(this.Cliente.NomeFantasia, 100); //NOM_FANT
            this.EscreverDado(this.Cliente.ContaFornecedorEBS, 8); //COD_CONT
            this.EscreverDado(this.Cliente.Localidade.Estado.Sigla); //UF
            this.EscreverDado(this.Cliente.CEP, 8); //CEP
            this.EscreverDado("", 5); //CX_POST
            this.EscreverDado("", 8); //CEP_CX_POST
            this.EscreverDado(this.Cliente.Telefone1, 12); //TEL
            this.EscreverDado("", 12); //FAX
            this.EscreverDado(this.Cliente.Email, 100); //EMAIL

            this.FinalizarRegistro();

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
