namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Grupo de informações do destinatário do CT-e
    /// </summary>
    public class Registro12400 : Registro
    {
        #region Construtores

        public Registro12400(string registro)
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

        public string ISUF { get; set; }

        public string email { get; set; }

        public Registro12410 enderDest { get; set; }

        public Registro12420 locEnt { get; set; }

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
            this.ISUF = this.ObterString(dados[5]);
            this.email = this.ObterString(dados[6]);
            this.ativ = this.ObterNumero(dados[7]);
        }

        #endregion
    }
}
