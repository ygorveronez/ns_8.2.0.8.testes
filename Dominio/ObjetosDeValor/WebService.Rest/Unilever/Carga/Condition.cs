using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "Condition", Namespace = "")]
	public class Condition
	{
		[XmlElement(ElementName = "item", Namespace = "")]
		public List<ItemCondition> ItemCondition { get; set; }
	}
}
