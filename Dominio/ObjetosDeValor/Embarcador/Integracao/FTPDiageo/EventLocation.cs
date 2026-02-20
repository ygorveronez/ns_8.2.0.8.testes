using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo
{
    [XmlRoot(ElementName = "EventLocation", Namespace = "")]
    public sealed class EventLocation
    {
        [XmlElement(ElementName = "ID", Namespace = "")]
        public string ID { get; set; }

        [XmlElement(ElementName = "Name", Namespace = "")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Address", Namespace = "")]
        public Address Address { get; set; }

        [XmlElement(ElementName = "Latitude", Namespace = "")]
        public double Latitude { get; set; }

        [XmlElement(ElementName = "Longitude", Namespace = "")]
        public double Longitude { get; set; }
    }
}
