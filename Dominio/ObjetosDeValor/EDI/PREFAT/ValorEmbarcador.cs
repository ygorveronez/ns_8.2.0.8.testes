namespace Dominio.ObjetosDeValor.EDI.PREFAT
{
    public class ValorEmbarcador
    {
        public decimal ValorFretePorPesoVolume { get; set; }
        public decimal ValorSECCAT { get; set; }
        public decimal ValorITR { get; set; }
        public decimal ValorPedagio { get; set; }
        public decimal ValoresDiversos { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorADEME { get; set; }
        public decimal PercentualTaxaISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal PercentualTaxaICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorFreteADVALOREM { get; set; }
        public decimal ValorDespacho { get; set; }
        public string Filler { get; set; }
    }
}
