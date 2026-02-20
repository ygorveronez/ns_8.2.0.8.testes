using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "Partner", Namespace = "")]
	public class Partner
	{
		[XmlElement(ElementName = "item", Namespace = "")]
		public List<ItemPartner> ItemPartner { get; set; }
	}
}
