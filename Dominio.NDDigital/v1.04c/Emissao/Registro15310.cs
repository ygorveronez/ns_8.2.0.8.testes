namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Documentos de transporte anterior em papel
    /// </summary>
    public class Registro15310 : Registro
    {
        #region Construtores

        public Registro15310(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public string CPF_CNPJ { get; set; }

        public string IE { get; set; }

        public string UF { get; set; }

        /// <summary>
        /// Nome / razão social
        /// </summary>
        public string xNome { get; set; }

        public Registro15320 idDocAnt { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.CPF_CNPJ = this.ObterString(dados[1]);
            this.IE = this.ObterString(dados[2]);
            this.UF = this.ObterString(dados[3]);
            this.xNome = this.ObterString(dados[4]);
        }

        #endregion
    }
}
