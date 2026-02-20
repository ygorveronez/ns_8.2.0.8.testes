using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.EMP
{
    public class Invoice
    {
        public string IssueDateTime { get; set; }
        public string InvoiceNumber { get; set; }
        public Recipient Recipient { get; set; }
        public ReceivingBank ReceivingBank { get; set; }
        public double TotalInvoiceValue { get; set; }
        public string ValueInWords { get; set; }
        public double TotalAdditions { get; set; }
        public double TotalDiscounts { get; set; }
        public List<Document> Documents { get; set; }
        public string Operator { get; set; }
    }

    public class Recipient
    {
        public string LegalName { get; set; }
        public string CustomerTax { get; set; }
        public string CustomerIE { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string PoBoxNumber { get; set; }
        public string UnitNumber { get; set; }
        public string District { get; set; }
        public string SubdivisionName { get; set; }
        public string PostalCode { get; set; }
    }

    public class ReceivingBank
    {
        public string LegalName { get; set; }
        public string Agency { get; set; }
        public string AccountNumber { get; set; }
        public string BankCode { get; set; }
    }

    public class Document
    {
        public double Value { get; set; }
        public int ItemQuantity { get; set; }
        public string ArrivalDatetime { get; set; }
        public List<CTEVessel> CteVessel { get; set; }
        public string BookingNumber { get; set; }
        public List<string> DocumentNumber { get; set; }
    }

    public class CTEVessel
    {
        public string VesselName { get; set; }
        public string VoyageNumber { get; set; }
        public string Direction { get; set; }
    }
}
