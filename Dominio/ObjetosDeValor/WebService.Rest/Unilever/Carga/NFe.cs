using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "Nfe", Namespace = "")]
	public class Nfe
	{
		[XmlElement(ElementName = "item", Namespace = "")]
		public List<ItemNfe> ItemNfe { get; set; }
	}

}
