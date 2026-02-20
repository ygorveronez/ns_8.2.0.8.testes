using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
    [XmlRoot(ElementName = "transmission", Namespace = "")]
    public sealed class Transmission
    {
        [XmlElement(ElementName = "message", Namespace = "")]
        public Message Message { get; set; }
    }
}
