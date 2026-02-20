using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit
{
    [XmlRoot(ElementName = "gswReceiverResponse", Namespace = "http://oracle.e1.bssv.JP55R001/")]
    public class GswReceiverResponse
    {
        [XmlElement(ElementName = "e1MessageList", Namespace = "")]
        public string E1MessageList { get; set; }

        [XmlElement(ElementName = "returnCod", Namespace = "")]
        public string ReturnCod { get; set; }

        [XmlElement(ElementName = "returnDesc", Namespace = "")]
        public string ReturnDesc { get; set; }

        [XmlElement(ElementName = "returnDescDet", Namespace = "")]
        public string ReturnDescDet { get; set; }
    }
}
