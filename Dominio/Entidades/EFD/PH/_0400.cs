namespace Dominio.Entidades.EFD.PH
{
    public class _0400 : RegistroPH
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

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
