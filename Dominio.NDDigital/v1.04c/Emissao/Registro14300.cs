namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações adicionais ao Fisco
    /// </summary>
    public class Registro14300 : Registro
    {
        #region Construtores

        public Registro14300(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Informações adicionais de interesse do Fisco
        /// </summary>
        public string infAdFisco { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.infAdFisco = this.ObterString(dados[1]);
        }

        #endregion
    }
}
