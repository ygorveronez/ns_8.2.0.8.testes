namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Simples Nacional
    /// </summary>
    public class Registro14210 : Registro
    {
        #region Construtores

        public Registro14210(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Indica se o contribuinte é Simples Nacional
        /// 1 - Sim
        /// </summary>
        public int indSN { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.indSN = this.ObterNumero(dados[1]);
        }

        #endregion
    }
}
