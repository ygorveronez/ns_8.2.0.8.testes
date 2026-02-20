namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Proprietário do veículo
    /// </summary>
    public class Registro16410 : Registro
    {
        #region Construtores

        public Registro16410(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public string CPF_CNPJ { get; set; }

        /// <summary>
        /// Registro Nacional de Transportadores Rodoviários de Carga
        /// </summary>
        public int RNTRC { get; set; }

        /// <summary>
        /// Razão Social
        /// </summary>
        public string xNome { get; set; }

        public string IE { get; set; }

        public string UF { get; set; }

        /// <summary>
        /// Tipo de Proprietário: 0 - TAC - Agregado, 1 - TAC - Independente, 2 - Outros
        /// </summary>
        public int tpProp { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.CPF_CNPJ = this.ObterString(dados[1]);
            this.RNTRC = this.ObterNumero(dados[2]);
            this.xNome = this.ObterString(dados[3]);
            this.IE = this.ObterString(dados[4]);
            this.UF = this.ObterString(dados[5]);
            this.tpProp = this.ObterNumero(dados[6]);
        }

        #endregion
    }
}
