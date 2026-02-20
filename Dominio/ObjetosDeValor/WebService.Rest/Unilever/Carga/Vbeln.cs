using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "Vbeln", Namespace = "")]
	public class Vbeln
	{
		[XmlElement(ElementName = "item", Namespace = "")]
		public List<string> ItemVbeln { get; set; }
	}
}
