namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Grupos de informações das Notas Fiscais Eletrõnicas
    /// </summary>
    public class Registro12130 : Registro
    {
        #region Construtores

        public Registro12130(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Chave de acesso da NF-e
        /// </summary>
        public string chave { get; set; }

        /// <summary>
        /// PIN SUFRAMA
        /// </summary>
        public int PIN { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.chave = this.ObterString(dados[1]);
            this.PIN = this.ObterNumero(dados[2]);
        }

        #endregion
    }
}
