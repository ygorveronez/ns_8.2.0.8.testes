namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Componentes do valor da prestação
    /// </summary>
    public class Registro23110 : Registro
    {
        #region Construtores

        public Registro23110(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Nome do componente
        /// </summary>
        public string xNome { get; set; }

        /// <summary>
        /// Valor do componente
        /// </summary>
        public decimal vComp { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.xNome = this.ObterString(dados[1]);
            this.vComp = this.ObterValor(dados[2]);
        }

        #endregion
    }
}
