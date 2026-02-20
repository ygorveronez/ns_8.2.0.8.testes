namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Documentos de transporte anterior eletrônicos
    /// </summary>
    public class Registro15322 : Registro
    {
        #region Construtores

        public Registro15322(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Chave de acesso do CT-e
        /// </summary>
        public string chave { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.chave = this.ObterString(dados[1]);
        }

        #endregion
    }
}
