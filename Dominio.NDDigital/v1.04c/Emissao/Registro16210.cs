namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Dados do emitente da OCC
    /// </summary>
    public class Registro16210 : Registro
    {
        #region Construtores

        public Registro16210(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public string CNPJ { get; set; }

        /// <summary>
        /// Código interno do emitente
        /// </summary>
        public string cInt { get; set; }

        public string IE { get; set; }

        public string UF { get; set; }

        public string fone { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.CNPJ = this.ObterString(dados[0]);
            this.cInt = this.ObterString(dados[1]);
            this.IE = this.ObterString(dados[2]);
            this.UF = this.ObterString(dados[3]);
            this.fone = this.ObterString(dados[4]);
        }

        #endregion
    }
}
