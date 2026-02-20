namespace Dominio.Entidades.EFD.SPED
{
    public class E990 : Registro
    {
        #region Construtores

        public E990(int totalLinhas)
            : base("E990")
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
