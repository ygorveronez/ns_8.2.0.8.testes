using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
    [XmlRoot(ElementName = "destinationWarehouseAddress", Namespace = "")]
    public sealed class DestinationWarehouseAddress
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "addressLine1")]
        public string AddressLine1 { get; set; }

        [XmlElement(ElementName = "addressLine2")]
        public string AddressLine2 { get; set; }

        [XmlElement(ElementName = "postalCode")]
        public string PostalCode { get; set; }

        [XmlElement(ElementName = "stateChoice")]
        public StateChoice StateChoice { get; set; }

    }
}
