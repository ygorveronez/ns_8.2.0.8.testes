using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "ExOutputTab", Namespace = "")]
	public class ExOutputTab
	{
		[XmlElement(ElementName = "item", Namespace = "")]
		public Item Item { get; set; }
	}
}
