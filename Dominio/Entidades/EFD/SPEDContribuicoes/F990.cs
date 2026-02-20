namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class F990 : Registro
    {
        #region Construtores

        public F990(int totalLinhas)
            : base("F990")
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
            this.EscreverDado(this.TotalLinhas); //QTD_LIN_F

            this.FinalizarRegistro();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
