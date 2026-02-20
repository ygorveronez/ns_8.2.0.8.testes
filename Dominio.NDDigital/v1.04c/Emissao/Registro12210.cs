namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Endereço do expedidor
    /// </summary>
    public class Registro12210 : Registro
    {
        #region Construtores

        public Registro12210(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

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
        /// Código município
        /// </summary>
        public int cMun { get; set; }

        /// <summary>
        /// Nome Município
        /// </summary>
        public string xMun { get; set; }

        public string CEP { get; set; }

        public string UF { get; set; }

        /// <summary>
        /// Código do País
        /// </summary>
        public int cPais { get; set; }

        /// <summary>
        /// Nome do País
        /// </summary>
        public string xPais { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.xLgr = this.ObterString(dados[1]);
            this.nro = this.ObterString(dados[2]);
            this.xCpl = this.ObterString(dados[3]);
            this.xBairro = this.ObterString(dados[4]);
            this.cMun = this.ObterNumero(dados[5]);
            this.xMun = this.ObterString(dados[6]);
            this.CEP = this.ObterString(dados[7]);
            this.UF = this.ObterString(dados[8]);
            this.cPais = this.ObterNumero(dados[9]);
            this.xPais = this.ObterString(dados[10]);
        }

        #endregion
    }
}
