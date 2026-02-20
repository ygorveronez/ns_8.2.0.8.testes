using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "item", Namespace = "")]
	public class ItemMessage2
	{
		[XmlElement(ElementName = "Message")]
		public string Message { get; set; }
	}
}
