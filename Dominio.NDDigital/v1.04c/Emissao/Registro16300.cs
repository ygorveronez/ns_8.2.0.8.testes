namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações de vale pedágio
    /// </summary>
    public class Registro16300 : Registro
    {
        #region Construtores

        public Registro16300(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// CNPJ da empresa fornecedora do Vale-Pedágio
        /// </summary>
        public string CNPJForn { get; set; }

        /// <summary>
        /// Número do comprovante de compra
        /// </summary>
        public string nCompra { get; set; }

        /// <summary>
        /// CNPJ do responsável pelo pagamento do Vale-Pedágio
        /// </summary>
        public string CNPJPg { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = dados[0];
            this.CNPJForn = dados[1];
            this.nCompra = dados[2];
            this.CNPJPg = dados[3];
        }

        #endregion
    }
}
