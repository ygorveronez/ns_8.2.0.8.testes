using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga.BookingAVRO
{
    public class PricingArchitectureAVRO
    {
        public decimal totalRate { get; set; }
        public List<PricingArchitectureCost> pricingArchitectureCostList { get; set; }
        public List<GpInformation> gpInformationList { get; set; }
    }
}
