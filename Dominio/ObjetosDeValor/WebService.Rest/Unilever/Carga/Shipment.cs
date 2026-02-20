using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "Shipment")]
	public class Shipment
	{
		[XmlElement(ElementName = "item")]
		public List<ItemShipment> ItemShipment { get; set; }
	}
}
