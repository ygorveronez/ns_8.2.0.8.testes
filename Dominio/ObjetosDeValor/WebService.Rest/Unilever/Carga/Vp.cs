using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "Vp", Namespace = "")]
	public class Vp
	{
		[XmlElement(ElementName = "item", Namespace = "")]
		public ItemVp ItemVp { get; set; }
	}
}
