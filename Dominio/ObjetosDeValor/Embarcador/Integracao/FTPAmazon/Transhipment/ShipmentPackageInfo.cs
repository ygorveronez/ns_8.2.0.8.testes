using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
    [XmlRoot(ElementName = "shipmentPackageInfo", Namespace = "")]
    public sealed class ShipmentPackageInfo
    {
        [XmlElement(ElementName = "cartonID")]
        public CartonID CartonID { get; set; }

        [XmlElement(ElementName = "shipmentPackageDeclaredGrossWeight")]
        public ShipmentPackageDeclaredGrossWeight ShipmentPackageDeclaredGrossWeight { get; set; }
    }
}
