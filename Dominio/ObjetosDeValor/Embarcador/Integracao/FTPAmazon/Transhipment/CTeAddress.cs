using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
    [XmlRoot(ElementName = "CTeAddress", Namespace = "")]
    public sealed class CTeAddress
    {
        [XmlElement(ElementName = "ALBaddressLine1")]
        public string ALBaddressLine1 { get; set; }

        [XmlElement(ElementName = "ALBaddressLine2")]
        public string ALBaddressLine2 { get; set; }

        [XmlElement(ElementName = "ALBzip")]
        public string ALBzip { get; set; }
    }
}
