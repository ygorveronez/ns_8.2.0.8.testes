namespace Dominio.Entidades.EFD.PH
{
    public class _9900 : RegistroPH
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
            this.ObterRegistrosPHDerivados();
            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
