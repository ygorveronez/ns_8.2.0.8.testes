using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
    [XmlRoot(ElementName = "nfeAddress", Namespace = "")]
    public sealed class NFeAddress
    {
        [XmlElement(ElementName = "Street")]
        public string Street { get; set; }

        [XmlElement(ElementName = "number")]
        public string Number { get; set; }

        [XmlElement(ElementName = "borough")]
        public string Borough { get; set; }

        [XmlElement(ElementName = "zipcode")]
        public string Zipcode { get; set; }

        [XmlElement(ElementName = "city")]
        public string City { get; set; }

        [XmlElement(ElementName = "state")]
        public string State { get; set; }
    }
}
