namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Dados da fatura
    /// </summary>
    public class Registro22052 : Registro
    {
        #region Construtores

        public Registro22052(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Número da fatura
        /// </summary>
        public string nFat { get; set; }

        /// <summary>
        /// Valor original da fatura
        /// </summary>
        public decimal vOrig { get; set; }

        /// <summary>
        /// Valor do desconto da fatura
        /// </summary>
        public decimal vDesc { get; set; }

        /// <summary>
        /// Valor líquido da fatura
        /// </summary>
        public decimal vLiq { get; set; }


        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.nFat = this.ObterString(dados[1]);
            this.vOrig = this.ObterValor(dados[2]);
            this.vDesc = this.ObterValor(dados[3]);
            this.vLiq = this.ObterValor(dados[4]);
        }

        #endregion
    }
}
