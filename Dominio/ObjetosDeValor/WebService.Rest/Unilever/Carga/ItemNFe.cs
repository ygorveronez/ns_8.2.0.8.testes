using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "item", Namespace = "")]
	public class ItemNfe
	{
		[XmlElement(ElementName = "Brgew")]
		public decimal Brgew { get; set; }

		[XmlElement(ElementName = "Vbeln")]
		public string Vbeln { get; set; }

		[XmlElement(ElementName = "Nfenum")]
		public int Nfenum { get; set; }

		[XmlElement(ElementName = "Series")]
		public string Series { get; set; }

		[XmlElement(ElementName = "Zfield")]
		public string Zfield { get; set; }

		[XmlElement(ElementName = "Stcd1")]
		public string Stcd1 { get; set; }

		[XmlElement(ElementName = "Stcd2")]
		public string Stcd2 { get; set; }

		[XmlElement(ElementName = "Stcd3")]
		public string Stcd3 { get; set; }

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
		public int ZdestCitcode { get; set; }

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

		[XmlElement(ElementName = "Stcd1Is")]
		public string Stcd1Is { get; set; }

		[XmlElement(ElementName = "Stcd2Is")]
		public string Stcd2Is { get; set; }

		[XmlElement(ElementName = "Stcd3Is")]
		public string Stcd3Is { get; set; }

		[XmlElement(ElementName = "Name1Is")]
		public string Name1Is { get; set; }

		[XmlElement(ElementName = "TelNumberIs")]
		public string TelNumberIs { get; set; }

		[XmlElement(ElementName = "AtwrtIs")]
		public string AtwrtIs { get; set; }

		[XmlElement(ElementName = "StreetIs")]
		public string StreetIs { get; set; }

		[XmlElement(ElementName = "HouseNum1Is")]
		public string HouseNum1Is { get; set; }

		[XmlElement(ElementName = "StrSuppl1Is")]
		public string StrSuppl1Is { get; set; }

		[XmlElement(ElementName = "City2Is")]
		public string City2Is { get; set; }

		[XmlElement(ElementName = "ZdestCitcodeIs")]
		public int ZdestCitcodeIs { get; set; }

		[XmlElement(ElementName = "City1Is")]
		public string City1Is { get; set; }

		[XmlElement(ElementName = "PostCode1Is")]
		public string PostCode1Is { get; set; }

		[XmlElement(ElementName = "RegionIs")]
		public string RegionIs { get; set; }

		[XmlElement(ElementName = "CountryIs")]
		public string CountryIs { get; set; }

		[XmlElement(ElementName = "CountryNameIs")]
		public string CountryNameIs { get; set; }

		[XmlElement(ElementName = "Docdat")]
		public string Docdat { get; set; }

		[XmlElement(ElementName = "Inco1")]
		public string Inco1 { get; set; }

		[XmlElement(ElementName = "Menge")]
		public string Menge { get; set; }

		[XmlElement(ElementName = "Shpunt")]
		public string Shpunt { get; set; }

		[XmlElement(ElementName = "Message1")]
		public string Message1 { get; set; }

		[XmlElement(ElementName = "NfeTotal")]
		public decimal NfeTotal { get; set; }

		[XmlElement(ElementName = "Matuse")]
		public string Matuse { get; set; }

		[XmlElement(ElementName = "Message2", Namespace = "")]
		public Message2 Message2 { get; set; }

		[XmlElement(ElementName = "CostRel")]
		public string CostRel { get; set; }

		[XmlElement(ElementName = "InbDocNum")]
		public string InbDocNum { get; set; }

		[XmlElement(ElementName = "OutbDocNum")]
		public string OutbDocNum { get; set; }

		[XmlElement(ElementName = "CnpjTake")]
		public string CnpjTake { get; set; }

		//[XmlElement(ElementName = "Pod", Namespace = "")]
		//public Pod Pod { get; set; }

		//[XmlElement(ElementName = "Partner", Namespace = "")]
		//public Partner Partner { get; set; }
	}
}
