using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
    [XmlRoot(ElementName = "manifestHeader", Namespace = "")]
    public sealed class ManifestHeader
    {
        [XmlElement(ElementName = "loadReferenceID")]
        public string LoadReferenceID { get; set; }

        [XmlElement(ElementName = "trailerName")]
        public string TrailerName { get; set; }

        [XmlElement(ElementName = "carrierInternalID")]
        public string CarrierInternalID { get; set; }

        [XmlElement(ElementName = "manifestNumber")]
        public string ManifestNumber { get; set; }

        [XmlElement(ElementName = "currencyCode")]
        public string CurrencyCode { get; set; }

        [XmlElement(ElementName = "warehouseLocationID")]
        public string WarehouseLocationID { get; set; }

        [XmlElement(ElementName = "shipFromAddress")]
        public ShipFromAddress ShipFromAddress { get; set; }

    }
}
