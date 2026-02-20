using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
	[XmlRoot(ElementName = "manifestDetail")]
	public class ManifestDetail
	{
		[XmlElement(ElementName = "shipmentDetail")]
		public List<ShipmentDetail> ShipmentDetail { get; set; }
	}
}
