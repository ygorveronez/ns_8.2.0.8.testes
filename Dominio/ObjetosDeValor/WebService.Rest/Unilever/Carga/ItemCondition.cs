using System.Xml.Serialization;


namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "item", Namespace = "")]
	public class ItemCondition
	{
		[XmlElement(ElementName = "Stunr")]
		public int Stunr { get; set; }

		[XmlElement(ElementName = "Kschl")]
		public string Kschl { get; set; }

		[XmlElement(ElementName = "Vtext")]
		public string Vtext { get; set; }

		[XmlElement(ElementName = "Kawrt")]
		public double Kawrt { get; set; }

		[XmlElement(ElementName = "Kmein")]
		public string Kmein { get; set; }

		[XmlElement(ElementName = "Kbetr")]
		public double Kbetr { get; set; }

		[XmlElement(ElementName = "Kwert")]
		public double Kwert { get; set; }

		[XmlElement(ElementName = "Waers")]
		public string Waers { get; set; }

		[XmlElement(ElementName = "Kinak")]
		public string Kinak { get; set; }
	}
}
