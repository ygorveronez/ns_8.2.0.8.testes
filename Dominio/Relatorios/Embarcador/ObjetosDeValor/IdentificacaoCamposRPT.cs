namespace Dominio.Relatorios.Embarcador.ObjetosDeValor
{
    public class IdentificacaoCamposRPT
    {
        public IdentificacaoCamposRPT()
        {
            this.GrupoFormula = "GrupoFormula";
            this.GrupoSort = "GrupoSort";
            this.GroupNameGrupoFormula = "GroupNameGrupoFormula1";
            this.GroupFooterSection = "GroupFooterSection1";
            this.GroupHeaderSection = "GroupHeaderSection1";
            this.SectionSumary = "Section4";
            this.ContadorRegistrosGrupo = "ContadorRegistrosGrupo";
            this.ContadorRegistrosTotal = "ContadorRegistrosTotal";
            this.AjustarAutomaticamenteLinhas = true;
            this.PrefixoCamposSum = "Somade";
            this.IndiceSumGroup = "2";
            this.IndiceSumReport = "1";
            this.IndiceData = "1";
            this.CorFundoNomeFormula = "CorFundo";
            this.CorFundoListra = "crSilver";
        }

        public string GrupoFormula { get; set; }
        public string GrupoSort { get; set; }
        public string GroupNameGrupoFormula { get; set; }
        public string GroupFooterSection { get; set; }
        public string GroupHeaderSection { get; set; }
        public string SectionSumary { get; set; }
        public string ContadorRegistrosGrupo { get; set; }
        public string ContadorRegistrosTotal { get; set; }
        public bool AjustarAutomaticamenteLinhas { get; set; }
        public string PrefixoCamposSum { get; set; }
        public string IndiceSumGroup { get; set; }
        public string IndiceSumReport { get; set; }
        public string IndiceData { get; set; }
        public string CorFundoNomeFormula { get; set; }
        public string CorFundoListra { get; set; }

    }
}
