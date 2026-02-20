namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class _1990 : Registro
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
            this.EscreverDado(this.TotalLinhas); //QTD_LIN_1

            this.FinalizarRegistro();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
