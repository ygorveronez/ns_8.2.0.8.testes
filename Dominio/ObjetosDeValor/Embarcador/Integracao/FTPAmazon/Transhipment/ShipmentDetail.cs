using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
	[XmlRoot(ElementName = "shipmentDetail")]
	public class ShipmentDetail
	{

		[XmlElement(ElementName = "containerId")]
		public string ContainerId { get; set; }

		[XmlElement(ElementName = "containerType")]
		public string ContainerType { get; set; }

		[XmlElement(ElementName = "destinationWarehouseLocationID")]
		public string DestinationWarehouseLocationID { get; set; }

		[XmlElement(ElementName = "destinationWarehouseAddress")]
		public DestinationWarehouseAddress DestinationWarehouseAddress { get; set; }

		[XmlElement(ElementName = "brNFes")]
		public List<BrNFe> BrNFes { get; set; }

        [XmlElement(ElementName = "brCTe")]
		public BrCTe BrCTe { get; set; }

		[XmlElement(ElementName = "shipmentPackageInfo")]
		public ShipmentPackageInfo ShipmentPackageInfo { get; set; }
	}
}
