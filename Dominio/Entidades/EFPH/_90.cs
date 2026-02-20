namespace Dominio.Entidades.EFPH
{
    public class _90 : Registro
    {
        #region Construtores

        public _90()
            : base("90")
        {
        }

        #endregion

        #region Propriedades

        public int TotalRegistros { get; set; }

        public decimal PesoTotal { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.TotalRegistros, 4, 2); //Quantidade Total de Registros Inclusive este
            this.EscreverDado("", 248); //Espaços

            this.FinalizarRegistro();

            this.ObterRegistrosEFPHDerivados();

            return this.RegistroEFPH.ToString();
        }

        #endregion
    }
}
