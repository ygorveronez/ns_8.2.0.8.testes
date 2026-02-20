namespace Dominio.ObjetosDeValor.EDI.DOCCOB
{
    public class DocumentoCobrancaImposto
    {
        public decimal ValorTotalICMS { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal ValorTotalISS { get; set; }
        public decimal AliquotaISS { get; set; }
        public decimal BaseCalculoISS { get; set; }
        public decimal ValorTotalICMSST { get; set; }
        public decimal AliquotaICMSST { get; set; }
        public decimal BaseCalculoICMSST { get; set; }
        public decimal ValorTotalIR { get; set; }
        public string Filler { get; set; }
    }
}
