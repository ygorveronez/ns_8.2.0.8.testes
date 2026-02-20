namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações do motorista (só preenchido em CT-e rodoviário de lotação)
    /// </summary>
    public class Registro16600 : Registro
    {
        #region Construtores

        public Registro16600(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Nome do motorista
        /// </summary>
        public string xNome { get; set; }

        /// <summary>
        /// CPF do motorista
        /// </summary>
        public string CPF { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.xNome = this.ObterString(dados[1]);
            this.CPF = this.ObterString(dados[2]);
        }

        #endregion
    }
}
