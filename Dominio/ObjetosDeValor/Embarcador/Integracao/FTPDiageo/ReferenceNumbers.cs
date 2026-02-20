using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo
{
    [XmlRoot(ElementName = "ReferenceNumbers", Namespace = "")]
    public sealed class ReferenceNumbers
    {
        public ReferenceNumber ReferenceNumber { get; set; }
    }

    public class ReferenceNumber
    {
        [XmlAttribute]
        public string type { get; set; }

        [XmlText]
        public string value { get; set; }
    }
}
