using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Gadle
{
    public class integracaoGadle
    {
        public string vehicleType { get; set; }
        public string scheduledFor { get; set; }
        public string notes { get; set; }
        public string externalId { get; set; }
        public decimal totalWeight { get; set; }
        public int driverAssistantsCount { get; set; }
        public Warehouse warehouse { get; set; }
        public List<Merchandise> merchandises { get; set; }
    }

    public class Address
    {
        public string postalCode { get; set; }
        public string number { get; set; }
        public string complement { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string thoroughfare { get; set; }
        public string neighborhood { get; set; }
        public string city { get; set; }
        public string state { get; set; }
    }

    public class TimeWindow
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    public class Shipper
    {
        public string name { get; set; }
        public string nationalDocumentNumber { get; set; }
    }

    public class Warehouse
    {
        public Address address { get; set; }
        public TimeWindow timeWindow { get; set; }
        public Shipper shipper { get; set; }
    }

    public class Recipient
    {
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public string nationalDocumentNumber { get; set; }
    }

    public class Merchandise
    {
        public string trackingCode { get; set; }
        public string notes { get; set; }
        public double totalWeight { get; set; }
        public int packagesAmount { get; set; }
        public Address address { get; set; }
        public TimeWindow timeWindow { get; set; }
        public Recipient recipient { get; set; }
    }
}
