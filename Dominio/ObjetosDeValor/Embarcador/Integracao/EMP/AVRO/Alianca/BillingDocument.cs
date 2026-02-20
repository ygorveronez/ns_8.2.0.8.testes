namespace Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.AVRO.Alianca
{
    public class BillingDocument
    {
        public BillingDocumentDetails BillingDocumentDetails { get; set; }
    }

    public class BillingDocumentDetails
    {
        public string Status { get; set; }
        public Shipper Shipper { get; set; }
        public Recipient Recipient { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
        public Instructions Instructions { get; set; }
        public string BarCode { get; set; }
    }

    public class Shipper
    {
        public string LegalName { get; set; }
        public string CustomerCode { get; set; }
    }

    public class Recipient
    {
        public string LegalName { get; set; }
        public string CustomerCode { get; set; }
        public PostalAddress PostalAddressCombined { get; set; }
    }

    public class PostalAddress
    {
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string PoBoxNumber { get; set; }
        public string UnitNumber { get; set; }
        public string District { get; set; }
    }

    public class PaymentDetails
    {
        public string PlaceOfPayment { get; set; }
        public string PaymentDate { get; set; }
        public string AgencyAssignorCode { get; set; }
        public string OurNumber { get; set; }
        public double TotalInvoiceValue { get; set; }
        public string IssueDateTime { get; set; }
        public string DocumentNumber { get; set; }
        public string DocType { get; set; }
        public string AcceptanceCode { get; set; }
        public string ProcessingDate { get; set; }
        public string Wallet { get; set; }
        public string CurrencyType { get; set; }
    }

    public class Instructions
    {
        public string DocumentNumber { get; set; }
        public string Booking { get; set; }
        public int ProtestDays { get; set; }
        public double InterestPerDelayedDay { get; set; }
        public double FineForDelay { get; set; }
    }
}
