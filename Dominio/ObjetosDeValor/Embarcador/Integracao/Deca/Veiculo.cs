using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Deca
{
    [XmlRoot(ElementName = "vehicle", Namespace = "")]
    public class Veiculo
    {
        [XmlElement(ElementName = "TransportationUnit", Namespace = "")]
        public string CodigoTransportador { get; set; }

        [XmlElement(ElementName = "Action", Namespace = "")]
        public string Action { get; set; }
    }
}
