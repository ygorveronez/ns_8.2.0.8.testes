namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao
{
    public class Taxis
    {
        public string cfopInbound { get; set; }
        public string taxType { get; set; }
        public decimal rate { get; set; }
        public double excludedBase { get; set; }
        public string cofinsLaw { get; set; }
        public string icmsLaw { get; set; }
        public string ipiLaw { get; set; }
        public string pisLaw { get; set; }
        public string issLaw { get; set; }
        public decimal taxValue { get; set; }
        public decimal? baseAmount { get; set; }
        public decimal? otherBase { get; set; }
    }
}
