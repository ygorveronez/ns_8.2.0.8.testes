namespace Dominio.Entidades.EFD.PH
{
    public class _9990 : RegistroPH
    {
        #region Construtores

        public _9990(int totalLinhas)
            : base("9990")
        {
            this.TotalLinhas = totalLinhas + 2;
        }

        #endregion

        #region Propriedades

        public int TotalLinhas { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            //this.EscreverDado(this.TotalLinhas);
            //this.FinalizarRegistro();
            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
