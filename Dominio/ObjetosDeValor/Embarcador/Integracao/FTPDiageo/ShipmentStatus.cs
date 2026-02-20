using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo
{
    [XmlRoot(ElementName = "ShipmentStatus", Namespace = "")]
    public sealed class ShipmentStatus
    {
        [XmlElement(ElementName = "Header", Namespace = "")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "StatusDetails", Namespace = "")]
        public StatusDetails StatusDetails { get; set; }
    }
}
