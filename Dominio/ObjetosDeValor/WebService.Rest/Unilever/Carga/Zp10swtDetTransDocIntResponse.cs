using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "Zp10swtDetTransDocIntResponse", Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
	public class Zp10swtDetTransDocIntResponse
	{
		[XmlElement(ElementName = "ExOutputTab", Namespace = "")]
		public ExOutputTab ExOutputTab { get; set; }
		//[XmlElement(ElementName = "Shipment", Namespace = "")]
		//public Shipment Shipment { get; set; }
		[XmlAttribute(AttributeName = "n0", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string N0 { get; set; }
	}

}
