namespace Dominio.Entidades.EFD.SPED
{
    public class _0190 : Registro
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

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
