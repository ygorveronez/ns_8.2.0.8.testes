using System.Xml.Serialization;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo
{
    [XmlRoot(ElementName = "Event", Namespace = "")]
    public sealed class Event
    {
        [XmlElement(ElementName = "EventCode", Namespace = "")]
        public string EventCode { get; set; }

        [XmlElement(ElementName = "EventDescription", Namespace = "")]
        public string EventDescription { get; set; }

        [XmlElement(ElementName = "EventDateTime", Namespace = "")]
        public DateTime EventDateTime { get; set; }

        [XmlElement(ElementName = "Exception", Namespace = "")]
        public Exception Exception { get; set; }
    }
}
