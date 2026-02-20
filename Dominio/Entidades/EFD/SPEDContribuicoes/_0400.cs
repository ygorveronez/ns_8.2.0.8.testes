namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class _0400 : Registro
    {
        #region Construtores

        public _0400()
            : base("0400")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.CFOP CFOP { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CFOP.CodigoCFOP, 4); //COD_NAT
            this.EscreverDado(this.CFOP.Descricao); //DESCR_NAT

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
