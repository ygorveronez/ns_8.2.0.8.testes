using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "item", Namespace = "")]
	public class ItemVp
	{
		[XmlElement(ElementName = "Truckplate")]
		public string Truckplate { get; set; }

		[XmlElement(ElementName = "Trailerplate")]
		public string Trailerplate { get; set; }

		[XmlElement(ElementName = "Btrainplate")]
		public string Btrainplate { get; set; }

		[XmlElement(ElementName = "Truckaxles")]
		public int Truckaxles { get; set; }

		[XmlElement(ElementName = "Traileraxles")]
		public int Traileraxles { get; set; }

		[XmlElement(ElementName = "Btrainaxles")]
		public int Btrainaxles { get; set; }

		[XmlElement(ElementName = "Truckufid")]
		public string Truckufid { get; set; }

		[XmlElement(ElementName = "Trailerufid")]
		public string Trailerufid { get; set; }

		[XmlElement(ElementName = "Btrainufid")]
		public string Btrainufid { get; set; }

		[XmlElement(ElementName = "Totalaxles")]
		public int Totalaxles { get; set; }

		[XmlElement(ElementName = "Expectedaxles")]
		public int Expectedaxles { get; set; }

		[XmlElement(ElementName = "Tagstatus")]
		public int Tagstatus { get; set; }

		[XmlElement(ElementName = "Exptollvalue")]
		public int Exptollvalue { get; set; }

		[XmlElement(ElementName = "Realtollvalue")]
		public double Realtollvalue { get; set; }

		[XmlElement(ElementName = "Vpstatus")]
		public int Vpstatus { get; set; }

		[XmlElement(ElementName = "Receiptnumber")]
		public string Receiptnumber { get; set; }

		[XmlElement(ElementName = "Tollvendor")]
		public string Tollvendor { get; set; }

		[XmlElement(ElementName = "Occurrencenum")]
		public int Occurrencenum { get; set; }

		[XmlElement(ElementName = "VpSuspendedaxle")]
		public string VpSuspendedaxle { get; set; }
	}
}
