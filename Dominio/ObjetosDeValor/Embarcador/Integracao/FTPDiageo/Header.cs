using System.Xml.Serialization;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo
{
    [XmlRoot(ElementName = "Header", Namespace = "")]
    public sealed class Header
    {
        [XmlElement(ElementName = "TransactionID", Namespace = "")]
        public string TransactionID { get; set; }

        [XmlElement(ElementName = "LoadNumber", Namespace = "")]
        public string LoadNumber { get; set; }

        [XmlElement(ElementName = "DateTime", Namespace = "")]
        public DateTime DateTime { get; set; }

        [XmlElement(ElementName = "CarrierCode", Namespace = "")]
        public string CarrierCode { get; set; }

        [XmlElement(ElementName = "CarrierReference", Namespace = "")]
        public string CarrierReference { get; set; }

        [XmlElement(ElementName = "SCAC", Namespace = "")]
        public string SCAC { get; set; }

        [XmlElement(ElementName = "Trailer", Namespace = "")]
        public string Trailer { get; set; }

        [XmlElement(ElementName = "Tractor", Namespace = "")]
        public string Tractor { get; set; }

        [XmlElement(ElementName = "Purpose", Namespace = "")]
        public string Purpose { get; set; }

        [XmlElement(ElementName = "MoveType", Namespace = "")]
        public string MoveType { get; set; }
    }
}
