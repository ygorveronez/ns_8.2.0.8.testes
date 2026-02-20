namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informar quando o tamador não é contribuinte de ICMS
    /// </summary>
    public class Registro22120 : Registro
    {
        #region Construtores

        public Registro22120(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Chave de acesso de CT-e de anulação
        /// </summary>
        public string refCTeAnu { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.refCTeAnu = this.ObterString(dados[1]);
        }

        #endregion
    }
}
