using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Dexco
{
    public class ReturnCreateFO
    {
        [XmlElement(ElementName = "AccessKey")]
        public string AccessKey { get; set; }

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "Message")]
        public string Message { get; set; }

        [XmlElement(ElementName = "FONumber")]
        public string  FONumber { get; set; }

    }
}
