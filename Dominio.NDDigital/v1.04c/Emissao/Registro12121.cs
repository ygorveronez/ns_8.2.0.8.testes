namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Grupo de informações do Local de retirada constante na NF
    /// </summary>
    public class Registro12121 : Registro
    {
        #region Construtores

        public Registro12121(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public string CPF_CNPJ { get; set; }

        /// <summary>
        /// Razão social
        /// </summary>
        public string xNome { get; set; }

        /// <summary>
        /// Logradouro
        /// </summary>
        public string xLgr { get; set; }

        /// <summary>
        /// Número
        /// </summary>
        public string nro { get; set; }

        /// <summary>
        /// Complemento
        /// </summary>
        public string xCpl { get; set; }

        public string xBairro { get; set; }

        /// <summary>
        /// Código Município
        /// </summary>
        public int cMun { get; set; }

        /// <summary>
        /// Nome Município
        /// </summary>
        public string xMun { get; set; }

        public string UF { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.CPF_CNPJ = this.ObterString(dados[1]);
            this.xNome = this.ObterString(dados[2]);
            this.xLgr = this.ObterString(dados[3]);
            this.nro = this.ObterString(dados[4]);
            this.xCpl = this.ObterString(dados[5]);
            this.xBairro = this.ObterString(dados[6]);
            this.cMun = this.ObterNumero(dados[7]);
            this.xMun = this.ObterString(dados[8]);
            this.UF = this.ObterString(dados[9]);
        }

        #endregion
    }
}
