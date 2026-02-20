namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações do expedidor da carga
    /// </summary>
    public class Registro12200 : Registro
    {
        #region Construtores

        public Registro12200(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public string CPF_CNPJ { get; set; }

        public string IE { get; set; }

        /// <summary>
        /// Nome/razão social
        /// </summary>
        public string xNome { get; set; }

        public string fone { get; set; }

        public string email { get; set; }

        public Registro12210 enderExped { get; set; }

        /// <summary>
        /// Atividade
        /// </summary>
        public int ativ { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.CPF_CNPJ = this.ObterString(dados[1]);
            this.IE = this.ObterString(dados[2]);
            this.xNome = this.ObterString(dados[3]);
            this.fone = this.ObterString(dados[4]);
            this.email = this.ObterString(dados[5]);
            this.ativ = this.ObterNumero(dados[6]);
        }

        #endregion
    }
}
