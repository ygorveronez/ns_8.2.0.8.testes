namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class M990 : Registro
    {
        #region Construtores

        public M990(int totalLinhas)
            : base("M990")
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
            this.EscreverDado(this.TotalLinhas); //QTD_LIN_M

            this.FinalizarRegistro();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
