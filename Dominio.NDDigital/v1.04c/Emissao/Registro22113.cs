namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Chave de acesso do NF-e
    /// </summary>
    public class Registro22113 : Registro
    {
        #region Construtores

        public Registro22113(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Chave de acesso do CT-e emitida pelo Tomador
        /// </summary>
        public string refCTe { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.refCTe = this.ObterString(dados[1]);
        }

        #endregion
    }
}
