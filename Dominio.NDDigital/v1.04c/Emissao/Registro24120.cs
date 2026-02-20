namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Prestação sujeito à tributação com redução de BC do ICMS
    /// </summary>
    public class Registro24120 : Registro
    {
        #region Construtores

        public Registro24120(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Tributação do Serviço
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
        }

        #endregion
    }
}
