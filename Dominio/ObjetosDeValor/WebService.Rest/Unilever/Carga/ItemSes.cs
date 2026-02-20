using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "item", Namespace = "")]
	public class ItemSes
	{

		[XmlElement(ElementName = "Lblni")]
		public string Lblni { get; set; }

		[XmlElement(ElementName = "Ebeln")]
		public string Ebeln { get; set; }

		[XmlElement(ElementName = "Bsart")]
		public string Bsart { get; set; }

		[XmlElement(ElementName = "Erdat")]
		public string Erdat { get; set; }

		[XmlElement(ElementName = "Aedat")]
		public string Aedat { get; set; }

		[XmlElement(ElementName = "Mwskz")]
		public string Mwskz { get; set; }

		[XmlElement(ElementName = "KwertIcmsPre")]
		public double KwertIcmsPre { get; set; }

		[XmlElement(ElementName = "KwertIcmsSt")]
		public double KwertIcmsSt { get; set; }

		[XmlElement(ElementName = "KwertIcmsVal")]
		public double KwertIcmsVal { get; set; }

		[XmlElement(ElementName = "KawrtIcms")]
		public double KawrtIcms { get; set; }

		[XmlElement(ElementName = "KbetrIcmsAli")]
		public double KbetrIcmsAli { get; set; }

		[XmlElement(ElementName = "KwertPisVal")]
		public double KwertPisVal { get; set; }

		[XmlElement(ElementName = "KawrtPis")]
		public double KawrtPis { get; set; }

		[XmlElement(ElementName = "KbetrPsAli")]
		public double KbetrPsAli { get; set; }

		[XmlElement(ElementName = "KwertCofinsVal")]
		public string KwertCofinsVal { get; set; }

		[XmlElement(ElementName = "KawrtCofins")]
		public double KawrtCofins { get; set; }

		[XmlElement(ElementName = "KbetrCofinsAli")]
		public string KbetrCofinsAli { get; set; }

		[XmlElement(ElementName = "KwertIssVal")]
		public double KwertIssVal { get; set; }

		[XmlElement(ElementName = "KawrtIss")]
		public double KawrtIss { get; set; }

		[XmlElement(ElementName = "KbetrIssAli")]
		public double KbetrIssAli { get; set; }

		[XmlElement(ElementName = "Zsimpleop")]
		public string Zsimpleop { get; set; }

		[XmlElement(ElementName = "Zbasered")]
		public double Zbasered { get; set; }

		[XmlElement(ElementName = "KbetrIcmsExe")]
		public string KbetrIcmsExe { get; set; }

		[XmlElement(ElementName = "Waers")]
		public string Waers { get; set; }

		[XmlElement(ElementName = "Ktext1")]
		public string Ktext1 { get; set; }

		[XmlElement(ElementName = "Fknum")]
		public string Fknum { get; set; }

		[XmlElement(ElementName = "Knttp")]
		public string Knttp { get; set; }

		[XmlElement(ElementName = "Kostl")]
		public string Kostl { get; set; }

		[XmlElement(ElementName = "Werks")]
		public string Werks { get; set; }

		[XmlElement(ElementName = "Sakto")]
		public string Sakto { get; set; }

		[XmlElement(ElementName = "Aufnr")]
		public string Aufnr { get; set; }

		[XmlElement(ElementName = "GrossValue")]
		public double GrossValue { get; set; }

		[XmlElement(ElementName = "NetValue")]
		public double NetValue { get; set; }

		[XmlElement(ElementName = "CalcStat")]
		public string CalcStat { get; set; }

		[XmlElement(ElementName = "AcntDetStat")]
		public string AcntDetStat { get; set; }

		[XmlElement(ElementName = "TransferStat")]
		public string TransferStat { get; set; }

		[XmlElement(ElementName = "CreateDate")]
		public string CreateDate { get; set; }
	}
}
