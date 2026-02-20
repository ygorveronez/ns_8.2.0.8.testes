namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Documentos de transporte anterior em papel
    /// </summary>
    public class Registro15320 : Registro
    {
        #region Construtores

        public Registro15320(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public Registro15321 idDocAntPap { get; set; }

        public Registro15322 idDocAntEle { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
        }

        #endregion
    }
}
