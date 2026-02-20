namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class NotaEntradaTrailler
    {
        public string TipoRegistro { get; set; }
        public decimal ValorContabil { get; set; }
        public decimal BasePIS { get; set; }
        public decimal BaseCOFINS { get; set; }
        public decimal BaseCSLL { get; set; }
        public decimal BaseIRPJ { get; set; }
        public decimal BaseICMSA { get; set; }
        public decimal ValorICMSA { get; set; }
        public decimal BaseICMSB { get; set; }
        public decimal ValorICMSB { get; set; }
        public decimal BaseICMSC { get; set; }
        public decimal ValorICMSC { get; set; }
        public decimal BaseICMSD { get; set; }
        public decimal ValorICMSD { get; set; }
        public decimal IsentaICMS { get; set; }
        public decimal OutraICMS { get; set; }
        public decimal BaseIPI { get; set; }
        public decimal ValorIPI { get; set; }
        public decimal IsentaIPI { get; set; }
        public decimal OutraIPI { get; set; }
        public decimal MercadoriaST { get; set; }
        public decimal BaseST { get; set; }
        public decimal ICMSST { get; set; }
        public decimal Difereida { get; set; }
        public decimal BaseICMSE { get; set; }
        public decimal ValorICMSE { get; set; }
        public decimal BaseICMSF { get; set; }
        public decimal ValorICMSF { get; set; }
        public string Brancos { get; set; }
        public string Sequencia { get; set; }
    }
}
