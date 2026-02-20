namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class D010 : Registro
    {
        #region Construtores

        public D010() : base("D010") { }

        #endregion

        #region Propriedades

        public Empresa Empresa { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Empresa.CNPJ, 14);

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
