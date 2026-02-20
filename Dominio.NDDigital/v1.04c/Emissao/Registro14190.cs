namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// ICMS outros
    /// </summary>
    public class Registro14190 : Registro
    {
        #region Construtores

        public Registro14190(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código de Situação Tributária
        /// </summary>
        public string CST { get; set; }

        /// <summary>
        /// Percentual de redução da BC do ICMS
        /// </summary>
        public decimal pRedBC { get; set; }

        /// <summary>
        /// Valor da BC do ICMS
        /// </summary>
        public decimal vBC { get; set; }

        /// <summary>
        /// Alíquota do ICMS
        /// </summary>
        public decimal pICMS { get; set; }

        /// <summary>
        /// Valor do ICMS
        /// </summary>
        public decimal vICMS { get; set; }

        /// <summary>
        /// Valor do Crédito outorgado/presumido
        /// </summary>
        public decimal vCred { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.CST = this.ObterString(dados[1]);
            this.pRedBC = this.ObterValor(dados[2]);
            this.vBC = this.ObterValor(dados[3]);
            this.pICMS = this.ObterValor(dados[4]);
            this.vICMS = this.ObterValor(dados[5]);
            this.vCred = this.ObterValor(dados[6]);
        }

        #endregion
    }
}
