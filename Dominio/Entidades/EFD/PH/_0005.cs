namespace Dominio.Entidades.EFD.PH
{
    public class _0005 : RegistroPH
    {
        #region Construtores

        public _0005()
            : base("0005")
        {
        }

        #endregion

        #region Propriedades

        public Empresa Empresa { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            System.Text.RegularExpressions.Regex regexNumbers = new System.Text.RegularExpressions.Regex("[^0-9]");

            this.EscreverDado(string.IsNullOrWhiteSpace(this.Empresa.NomeFantasia) ? this.Empresa.RazaoSocial : this.Empresa.NomeFantasia, 60); //FANTASIA
            this.EscreverDado(this.Empresa.CEP);
            this.EscreverDado(this.Empresa.Endereco, 60);
            this.EscreverDado(this.Empresa.Numero, 10);
            this.EscreverDado(this.Empresa.Complemento, 60);
            this.EscreverDado(this.Empresa.Bairro, 60);
            this.EscreverDado(string.IsNullOrWhiteSpace(this.Empresa.Telefone) ? string.Empty : regexNumbers.Replace(this.Empresa.Telefone, ""));
            this.EscreverDado(string.IsNullOrWhiteSpace(this.Empresa.Fax) ? string.Empty : regexNumbers.Replace(this.Empresa.Fax, ""));
            this.EscreverDado(this.Empresa.EmailAdministrativo);

            this.FinalizarRegistro();

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
