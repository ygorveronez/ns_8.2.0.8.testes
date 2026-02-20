namespace Dominio.Entidades.EFD.PH
{
    public class _1990 : RegistroPH
    {
        #region Construtores

        public _1990(int totalLinhas)
            : base("1990")
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
