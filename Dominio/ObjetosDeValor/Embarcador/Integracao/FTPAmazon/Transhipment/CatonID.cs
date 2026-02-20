using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
	[XmlRoot(ElementName = "cartonID")]
	public class CartonID
	{
		[XmlElement(ElementName = "amazonBarCode")]
		public string AmazonBarCode { get; set; }

		[XmlElement(ElementName = "encryptedShipmentID")]
		public string EncryptedShipmentID { get; set; }

		[XmlElement(ElementName = "packageID")]
		public string PackageID { get; set; }

		[XmlElement(ElementName = "trackingID")]
		public string TrackingID { get; set; }
	}
}
