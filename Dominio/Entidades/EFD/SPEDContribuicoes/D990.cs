namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class D990 : Registro
    {
        #region Construtores

        public D990(int totalLinhas)
            : base("D990")
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

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
