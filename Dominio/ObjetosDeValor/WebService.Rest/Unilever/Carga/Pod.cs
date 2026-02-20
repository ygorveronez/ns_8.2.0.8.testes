using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "Pod", Namespace = "")]
	public class Pod
	{
		[XmlElement(ElementName = "item", Namespace = "")]
		public ItemPod ItemPod { get; set; }
	}
}
