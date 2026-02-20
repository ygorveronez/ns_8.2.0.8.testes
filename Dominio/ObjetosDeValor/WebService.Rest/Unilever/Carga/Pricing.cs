using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "Pricing", Namespace = "")]
	public class Pricing
	{
		[XmlElement(ElementName = "Condition", Namespace = "")]
		public Condition Condition { get; set; }
	}
}
