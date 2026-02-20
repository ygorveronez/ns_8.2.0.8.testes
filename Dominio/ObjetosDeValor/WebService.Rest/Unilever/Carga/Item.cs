using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "item", Namespace = "")]
	public class Item
	{
		[XmlElement(ElementName = "Tknum")]
		public string Tknum { get; set; }

		[XmlElement(ElementName = "Shtyp")]
		public string Shtyp { get; set; }

		[XmlElement(ElementName = "Edocscte")]
		public string Edocscte { get; set; }

		[XmlElement(ElementName = "Tpp")]
		public string Tpp { get; set; }

		[XmlElement(ElementName = "Driverid")]
		public string Driverid { get; set; }

		[XmlElement(ElementName = "Onecte")]
		public int Onecte { get; set; }

		[XmlElement(ElementName = "CarrCode")]
		public int CarrCode { get; set; }

		[XmlElement(ElementName = "GlobalStat")]
		public int GlobalStat { get; set; }

		[XmlElement(ElementName = "Stage", Namespace = "")]
		public Stage Stage { get; set; }

		[XmlElement(ElementName = "Message")]
		public string Message { get; set; }
	}
}
