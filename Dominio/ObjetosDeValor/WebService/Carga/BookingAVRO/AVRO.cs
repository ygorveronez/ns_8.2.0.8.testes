using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga.BookingAVRO
{
    public class AVRO
    {
        public int bookingId { get; set; }
        public string bookingNumber { get; set; }
        public string bookingStatus { get; set; }
        public string bookingCustomerType { get; set; }
        public string bookingResponsibleForPayment { get; set; }
        public int agreementId { get; set; }
        public string agreementNumber { get; set; }
        public AgreementCorporateGroup agreementCorporateGroup { get; set; }
        public Customer agreementCustomer { get; set; }
        public Customer bookingShipper { get; set; }
        public List<LegBooking> legBookingList { get; set; }
        public List<ProductList> productList { get; set; }
        public Equipment equipment { get; set; }
        public SpecialCargo specialCargo { get; set; }
        public PricingArchitectureAVRO pricingArchitectureAVRO { get; set; }
    }
}
