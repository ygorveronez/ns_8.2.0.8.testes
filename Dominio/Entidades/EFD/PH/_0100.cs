namespace Dominio.Entidades.EFD.PH
{
    public class _0100 : RegistroPH
    {
        #region Construtores

        public _0100()
            : base("0100")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.Cliente Contador { get; set; }

        public string CRC { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Contador.Nome, 100); //NOME
            this.EscreverDado(this.Contador.Tipo == "F" ? this.Contador.CPF_CNPJ_SemFormato : string.Empty, 11); //CPF
            this.EscreverDado(this.CRC, 15); //CRC
            this.EscreverDado(this.Contador.Tipo == "J" ? this.Contador.CPF_CNPJ_SemFormato : string.Empty, 14); //CNPJ
            this.EscreverDado(Utilidades.String.OnlyNumbers(this.Contador.CEP), 8); //CEP
            this.EscreverDado(this.Contador.Endereco, 60); //END
            this.EscreverDado(this.Contador.Numero, 10); //NUM
            this.EscreverDado(this.Contador.Complemento, 60); //COMPL
            this.EscreverDado(this.Contador.Bairro, 60); //BAIRRO
            this.EscreverDado(Utilidades.String.OnlyNumbers(this.Contador.Telefone1), 11); //FONE
            this.EscreverDado(Utilidades.String.OnlyNumbers(this.Contador.Telefone2), 11); //FAX
            this.EscreverDado(this.Contador.Email, 500); //EMAIL
            this.EscreverDado(this.Contador.Localidade.CodigoIBGE, 7); //COD_MUN

            this.FinalizarRegistro();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
