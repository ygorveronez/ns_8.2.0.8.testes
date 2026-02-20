namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class C990 : Registro
    {
        #region Construtores

        public C990(int totalLinhas)
            : base("C990")
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
            this.EscreverDado(this.TotalLinhas); //QTD_LIN_C

            this.FinalizarRegistro();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
