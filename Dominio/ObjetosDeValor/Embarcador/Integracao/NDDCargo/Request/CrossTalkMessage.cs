using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "CrossTalk_Message", Namespace = "http://www.nddigital.com.br/nddcargo")]
    public class CrossTalkMessage<T>
    {
        [XmlElement(ElementName = "CrossTalk_Header")]
        public T Header { get; set; }

        [XmlElement(ElementName = "CrossTalk_Body")]
        public CrossTalkBody Body { get; set; }
    }
}
