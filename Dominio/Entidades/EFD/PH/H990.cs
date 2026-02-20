namespace Dominio.Entidades.EFD.PH
{
    public class H990 : RegistroPH
    {
        #region Construtores

        public H990(int totalLinhas)
            : base("H990")
        {
            this.TotalLinhas = totalLinhas + 1;
        }

        #endregion

        #region Propriedades

        public int TotalLinhas { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.TotalLinhas);
            this.FinalizarRegistro();
            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
