namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações do modal
    /// </summary>
    public class Registro15900 : Registro
    {
        #region Construtores

        public Registro15900(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Versão do leiaute específico para o Modal
        /// </summary>
        public string versaoModal { get; set; }

        public Registro16000 rodo { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.versaoModal = this.ObterString(dados[1]);
        }

        #endregion
    }
}
