namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class F010 : Registro
    {
        #region Construtores

        public F010() : base("F010") { }

        #endregion

        #region Propriedades

        public Dominio.Entidades.Empresa Empresa { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Empresa.CNPJ, 14); //CNPJ

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
