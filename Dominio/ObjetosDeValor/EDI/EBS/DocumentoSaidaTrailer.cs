namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class DocumentoSaidaTrailer
    {
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
        public decimal IsentasICMS { get; set; }
        public decimal OutrasICMS { get; set; }
        public decimal BaseIPI { get; set; }
        public decimal ValorIPI { get; set; }
        public decimal MercadoriasST { get; set; }
        public decimal BaseST { get; set; }
        public decimal ICMSST { get; set; }
        public decimal Diferidas { get; set; }
        public decimal BaseISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal IsentasISS { get; set; }
        public decimal IRRFISS { get; set; }
        public int Sequencia { get; set; }
    }
}
