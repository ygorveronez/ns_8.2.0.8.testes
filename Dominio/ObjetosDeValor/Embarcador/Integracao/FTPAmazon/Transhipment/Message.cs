using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
    [XmlRoot(ElementName = "message", Namespace = "")]
    public sealed class Message
    {
        [XmlElement(ElementName = "amazonManifest", Namespace = "")]
        public AmazonManifest AmazonManifest { get; set; }
    }
}
