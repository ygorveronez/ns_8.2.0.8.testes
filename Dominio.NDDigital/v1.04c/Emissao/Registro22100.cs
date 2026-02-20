namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações adicionais do CT-e de substituição
    /// </summary>
    public class Registro22100 : Registro
    {
        #region Construtores

        public Registro22100(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Chave do CT-e substituído
        /// </summary>
        public string chCTe { get; set; }

        public Registro22110 tomaICMS { get; set; }

        public Registro22120 tomaNaoICMS { get; set; }
        
        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.chCTe = this.ObterString(dados[1]);
        }

        #endregion
    }
}
