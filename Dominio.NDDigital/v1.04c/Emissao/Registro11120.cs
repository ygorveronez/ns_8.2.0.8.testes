namespace Dominio.NDDigital.v104.Emissao
{
    public class Registro11120 : Registro
    {
        #region Construtores

        public Registro11120(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// 4 - Outros
        /// </summary>
        public int toma { get; set; }

        public string CPF_CNPJ { get; set; }

        public string IE { get; set; }

        /// <summary>
        /// Razão social
        /// </summary>
        public string xNome { get; set; }

        /// <summary>
        /// Nome fantasia
        /// </summary>
        public string xFant { get; set; }

        public string fone { get; set; }

        public string email { get; set; }

        public Registro11121 enderToma { get; set; }

        public int ativ { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.toma = this.ObterNumero(dados[1]);
            this.CPF_CNPJ = this.ObterString(dados[2]);
            this.IE = this.ObterString(dados[3]);
            this.xNome = this.ObterString(dados[4]);
            this.xFant = this.ObterString(dados[5]);
            this.fone = this.ObterString(dados[6]);
            this.email = this.ObterString(dados[7]);
            this.ativ = this.ObterNumero(dados[8]);
        }

        #endregion
    }
}
