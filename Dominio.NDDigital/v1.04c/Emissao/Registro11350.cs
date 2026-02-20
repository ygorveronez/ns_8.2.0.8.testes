namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Observação do Fisco
    /// </summary>
    public class Registro11350 : Registro
    {
        #region Construtores

        public Registro11350(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificação do campo
        /// </summary>
        public string xCampo { get; set; }

        /// <summary>
        /// Conteúdo do campo
        /// </summary>
        public string xTexto { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.xCampo = this.ObterString(dados[1]);
            this.xTexto = this.ObterString(dados[2]);
        }

        #endregion
    }
}
