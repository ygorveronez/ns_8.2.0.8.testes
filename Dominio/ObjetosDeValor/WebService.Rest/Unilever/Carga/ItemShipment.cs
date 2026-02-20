using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "item")]
	public class ItemShipment
	{
		[XmlElement(ElementName = "Tknum")]
		public int Tknum { get; set; }
	}
}
