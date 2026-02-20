namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class C191 : Registro
    {
        #region Construtores

        public C191() : base("C191") { }

        #endregion

        #region Propriedades

        public string CPFCNPJFornecedor { get; set; }

        public string CSTPIS { get; set; }

        public string CFOP { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {           
            return string.Empty;
        }

        #endregion
    }
}
