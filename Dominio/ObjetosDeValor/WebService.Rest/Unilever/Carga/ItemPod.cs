using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "item", Namespace = "")]
	public class ItemPod
	{
		[XmlElement(ElementName = "Temppoddate")]
		public string Temppoddate { get; set; }

		[XmlElement(ElementName = "Temppodtime")]
		public string Temppodtime { get; set; }

		[XmlElement(ElementName = "Temppoddoctype")]
		public string Temppoddoctype { get; set; }

		[XmlElement(ElementName = "Temppoddoc")]
		public string Temppoddoc { get; set; }

		[XmlElement(ElementName = "Defpoddate")]
		public string Defpoddate { get; set; }

		[XmlElement(ElementName = "Defpodtime")]
		public string Defpodtime { get; set; }

		[XmlElement(ElementName = "Defpoddoctype")]
		public string Defpoddoctype { get; set; }

		[XmlElement(ElementName = "Defpoddoc")]
		public string Defpoddoc { get; set; }
	}
}
