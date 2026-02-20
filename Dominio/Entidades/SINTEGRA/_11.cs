namespace Dominio.Entidades.SINTEGRA
{
    public class _11 : Registro
    {
        #region Construtores

        public _11()
            : base("11")
        {
        }

        #endregion

        #region Propriedades

        public Empresa Empresa { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Empresa.Endereco, 34); //Logradouro

            int numero = 0;
            int.TryParse(this.Empresa.Numero, out numero);

            this.EscreverDado(numero, 5); //Número
            this.EscreverDado(this.Empresa.Complemento, 22); //Complemento
            this.EscreverDado(this.Empresa.Bairro, 15); //Bairro
            this.EscreverDado(Utilidades.String.OnlyNumbers(this.Empresa.CEP), 8); //CEP
            this.EscreverDado(this.Empresa.Contato, 28); //Nome do Contato

            if (!string.IsNullOrWhiteSpace(this.Empresa.TelefoneContato))
            {
                long telefone = 0L;
                long.TryParse(Utilidades.String.OnlyNumbers(this.Empresa.TelefoneContato), out telefone);

                this.EscreverDado(telefone, 12); //Telefone
            }
            else
            {
                this.EscreverDado(0, 12); //Telefone
            }

            this.FinalizarRegistro();

            this.ObterRegistrosSINTEGRADerivados();

            return this.RegistroSINTEGRA.ToString();
        }

        #endregion
    }
}
