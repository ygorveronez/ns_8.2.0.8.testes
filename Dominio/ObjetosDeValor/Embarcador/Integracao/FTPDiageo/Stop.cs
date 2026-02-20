using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo
{
    [XmlRoot(ElementName = "Stop", Namespace = "")]
    public sealed class Stop
    {
        [XmlElement(ElementName = "StopNumber", Namespace = "")]
        public int StopNumber { get; set; }

        [XmlElement(ElementName = "SequenceNumber", Namespace = "")]
        public int SequenceNumber { get; set; }

        [XmlElement(ElementName = "Event", Namespace = "")]
        public Event Event { get; set; }

        [XmlElement(ElementName = "EventLocation", Namespace = "")]
        public EventLocation EventLocation { get; set; }
    }
}
