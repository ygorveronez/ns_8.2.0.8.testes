namespace Dominio.ObjetosDeValor.WebService.Carga.BookingAVRO
{
    public class PricingArchitectureCost
    {
        public int costId { get; set; }
        public string costName { get; set; }
        public string characteristic { get; set; }
        public string currency { get; set; }
        public string associatedProduct { get; set; }
        public decimal? costValue { get; set; }
        public decimal? markup { get; set; }
        public decimal? pisCofins { get; set; }
        public decimal? totalCost { get; set; }
        public string costType { get; set; }
    }
}
