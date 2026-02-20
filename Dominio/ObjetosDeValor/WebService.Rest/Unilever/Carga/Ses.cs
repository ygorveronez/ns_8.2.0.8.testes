using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "Ses", Namespace = "")]
	public class Ses
	{
		[XmlElement(ElementName = "item", Namespace = "")]
		public ItemSes ItemSes { get; set; }
	}
}
