namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Chave de acesso do NF-e
    /// </summary>
    public class Registro22111 : Registro
    {
        #region Construtores

        public Registro22111(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Chave de acesso da NF-e emitida pelo Tomador
        /// </summary>
        public string refNFe { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.refNFe = this.ObterString(dados[1]);
        }

        #endregion
    }
}
