using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "Message2", Namespace = "")]
	public class Message2
	{
		[XmlElement(ElementName = "item", Namespace = "")]
		public List<ItemMessage2> ItemMessage2 { get; set; }
	}
}
