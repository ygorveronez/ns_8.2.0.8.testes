namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Grupo de lacres
    /// </summary>
    public class Registro16500 : Registro
    {
        #region Construtores

        public Registro16500(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Número do Lacre
        /// </summary>
        public string nLacre { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.nLacre = this.ObterString(dados[1]);
        }

        #endregion
    }
}
