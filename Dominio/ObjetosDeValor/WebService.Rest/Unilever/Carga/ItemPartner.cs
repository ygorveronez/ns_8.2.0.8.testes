using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "item", Namespace = "")]
	public class ItemPartner
	{
		[XmlElement(ElementName = "Parvw")]
		public string Parvw { get; set; }

		[XmlElement(ElementName = "Stcd1")]
		public double Stcd1 { get; set; }

		[XmlElement(ElementName = "Stcd2")]
		public string Stcd2 { get; set; }

		[XmlElement(ElementName = "Stcd3")]
		public double Stcd3 { get; set; }

		[XmlElement(ElementName = "Name1")]
		public string Name1 { get; set; }

		[XmlElement(ElementName = "TelNumber")]
		public string TelNumber { get; set; }

		[XmlElement(ElementName = "Atwrt")]
		public string Atwrt { get; set; }

		[XmlElement(ElementName = "Street")]
		public string Street { get; set; }

		[XmlElement(ElementName = "HouseNum1")]
		public string HouseNum1 { get; set; }

		[XmlElement(ElementName = "StrSuppl1")]
		public string StrSuppl1 { get; set; }

		[XmlElement(ElementName = "City2")]
		public string City2 { get; set; }

		[XmlElement(ElementName = "ZdestCitcode")]
		public string ZdestCitcode { get; set; }

		[XmlElement(ElementName = "City1")]
		public string City1 { get; set; }

		[XmlElement(ElementName = "PostCode1")]
		public string PostCode1 { get; set; }

		[XmlElement(ElementName = "Region")]
		public string Region { get; set; }

		[XmlElement(ElementName = "Country")]
		public string Country { get; set; }

		[XmlElement(ElementName = "CountryName")]
		public string CountryName { get; set; }
	}
}
