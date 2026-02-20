namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações dos produtos perigosos
    /// </summary>
    public class Registro21000 : Registro
    {
        #region Construtores

        public Registro21000(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Número ONU
        /// </summary>
        public int nONU { get; set; }

        /// <summary>
        /// Ver a legislação de transporte de produtos perigosos aplicadas ao modal
        /// </summary>
        public string xNomeAE { get; set; }

        /// <summary>
        /// Classe ou subclasse, e risco subsidiário
        /// </summary>
        public string xClaRisco { get; set; }

        /// <summary>
        /// Grupo de Embalagem
        /// </summary>
        public string grEmb { get; set; }

        /// <summary>
        /// Quantidade total do produto
        /// </summary>
        public string qTotProd { get; set; }

        /// <summary>
        /// Quantide e tipo de volumes
        /// </summary>
        public string qVolTipo { get; set; }

        /// <summary>
        /// Ponto de fulgor
        /// </summary>
        public string pontoFulgor { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.nONU = this.ObterNumero(dados[1]);
            this.xNomeAE = this.ObterString(dados[2]);
            this.xClaRisco = this.ObterString(dados[3]);
            this.grEmb = this.ObterString(dados[4]);
            this.qTotProd = this.ObterString(dados[5]);
            this.qVolTipo = this.ObterString(dados[6]);
            this.pontoFulgor = this.ObterString(dados[7]);
        }

        #endregion
    }
}
