using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo
{
    [XmlRoot(ElementName = "StatusDetails", Namespace = "")]
    public sealed class StatusDetails
    {
        [XmlElement(ElementName = "ReferenceNumbers", Namespace = "")]
        public ReferenceNumbers ReferenceNumbers { get; set; }

        [XmlElement(ElementName = "Stop", Namespace = "")]
        public Stop Stop { get; set; }
    }
}
