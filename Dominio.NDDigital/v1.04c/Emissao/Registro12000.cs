namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações do emitente do conhecimento
    /// </summary>
    public class Registro12000 : Registro
    {
        #region Construtores

        public Registro12000(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public string CNPJ { get; set; }

        /// <summary>
        /// Inscrição Estadual
        /// </summary>
        public string IE { get; set; }

        /// <summary>
        /// Razão social
        /// </summary>
        public string xNome { get; set; }

        /// <summary>
        /// Nome fantasia
        /// </summary>
        public string xFant { get; set; }

        public Registro12010 enderEmit { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.CNPJ = this.ObterString(dados[1]);
            this.IE = this.ObterString(dados[2]);
            this.xNome = this.ObterString(dados[3]);
            this.xFant = this.ObterString(dados[4]);
        }

        #endregion
    }
}
