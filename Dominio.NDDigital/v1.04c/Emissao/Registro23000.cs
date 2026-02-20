namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações do Cte complementado
    /// </summary>
    public class Registro23000 : Registro
    {
        #region Construtores

        public Registro23000(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Chave do CT-e complementado
        /// </summary>
        public string chave { get; set; }

        public Registro23100 vPresComp { get; set; }

        public Registro24000 impComp { get; set; }
        
        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.chave = this.ObterString(dados[1]);
        }

        #endregion
    }
}
