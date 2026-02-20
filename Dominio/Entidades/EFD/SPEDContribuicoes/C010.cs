namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class C010 : Registro
    {
        #region Construtores

        public C010() : base("C010") { }

        #endregion

        #region Propriedades

        public Dominio.Entidades.Empresa Empresa { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Empresa.CNPJ); //CNPJ
            
            this.EscreverDado("2"); //IND_ESCRI
            
            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
