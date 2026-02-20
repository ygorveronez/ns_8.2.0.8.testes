namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// ICMS isento não tributado ou diferido
    /// </summary>
    public class Registro14145 : Registro
    {
        #region Construtores

        public Registro14145(string registro)
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

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.CST = this.ObterString(dados[1]);
        }

        #endregion
    }
}
