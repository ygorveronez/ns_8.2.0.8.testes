using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class Transportador
    {
        [XmlElement(ElementName = "rntrc")]
        public string Rntrc { get; set; }

        [XmlElement(ElementName = "cnpjTransportador")]
        public string CnpjTransportador { get; set; }

        [XmlElement(ElementName = "cpfTransportador")]
        public string CpfTransportador { get; set; }
    }
}
