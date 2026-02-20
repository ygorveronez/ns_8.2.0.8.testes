namespace Dominio.Entidades.EFD.PH
{
    public class _0190 : RegistroPH
    {
        #region Construtores

        public _0190()
            : base("0190")
        {
        }

        #endregion

        #region Propriedades

        public UnidadeMedidaGeral UnidadeMedida { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.UnidadeMedida.Sigla, 6); //UNID
            this.EscreverDado(this.UnidadeMedida.Descricao); //DESCR

            this.FinalizarRegistro();

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
