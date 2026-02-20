using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "Body")]
    public class Body
    {
        [XmlElement(ElementName = "mensagens")]
        public Mensagens Mensagens { get; set; }
    }
}
