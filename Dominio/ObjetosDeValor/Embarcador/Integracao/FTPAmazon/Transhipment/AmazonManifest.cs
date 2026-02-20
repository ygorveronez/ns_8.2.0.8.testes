using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
    [XmlRoot(ElementName = "amazonManifest", Namespace = "")]
    public sealed class AmazonManifest
    {
        [XmlElement(ElementName = "manifestHeader", Namespace = "")]
        public ManifestHeader ManifestHeader { get; set; }

        [XmlArray(ElementName = "manifestDetail"), XmlArrayItem(ElementName = "shipmentDetail")]
        public List<ShipmentDetail> ShipmentDetail { get; set; }
    }
}
