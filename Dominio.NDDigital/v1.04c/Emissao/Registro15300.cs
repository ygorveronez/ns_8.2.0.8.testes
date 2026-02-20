namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Documentos de transporte anterior em papel
    /// </summary>
    public class Registro15300 : Registro
    {
        #region Construtores

        public Registro15300(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public Registro15310 emiDocAnt { get; set; }

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
