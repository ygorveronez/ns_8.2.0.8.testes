using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
	[XmlRoot(ElementName = "shipmentPackageDeclaredGrossWeight")]
	public class ShipmentPackageDeclaredGrossWeight
	{
		[XmlElement(ElementName = "weightValue")]
		public decimal WeightValue { get; set; }
	}
}
