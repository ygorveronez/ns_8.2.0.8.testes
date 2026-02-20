using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "Stage", Namespace = "")]
	public class Stage
	{
		[XmlElement(ElementName = "item", Namespace = "")]
		public List<ItemStage> ItemStage { get; set; }
	}
}
