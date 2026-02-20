using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo
{
    [XmlRoot(ElementName = "Address", Namespace = "")]
    public sealed class Address
    {
        [XmlElement(ElementName = "AddressLineOne", Namespace = "")]
        public string AddressLineOne { get; set; }

        [XmlElement(ElementName = "AddressLineTwo", Namespace = "")]
        public string AddressLineTwo { get; set; }

        [XmlElement(ElementName = "City", Namespace = "")]
        public string City { get; set; }

        [XmlElement(ElementName = "StateOrProvince", Namespace = "")]
        public string StateOrProvince { get; set; }

        [XmlElement(ElementName = "Country", Namespace = "")]
        public string Country { get; set; }

        [XmlElement(ElementName = "PostalCode", Namespace = "")]
        public string PostalCode { get; set; }
    }
}
