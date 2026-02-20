namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class _9900 : Registro
    {
        #region Construtores

        public _9900()
            : base("9900")
        {
        }

        #endregion

        #region Propriedades

        public string Registro { get; set; }

        public int Total { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Registro);

            this.EscreverDado(this.Total);

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
