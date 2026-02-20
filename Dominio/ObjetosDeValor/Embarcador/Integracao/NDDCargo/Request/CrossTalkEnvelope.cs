using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "Envelope")]
    public class CrossTalkEnvelope<T>
    {
        [XmlElement(ElementName = "CrossTalk_Header")]
        public CrossTalkHeaderResponse CrossTalkHeader { get; set; }

        [XmlElement(ElementName = "CrossTalk_Body")]
        public T CrossTalkBody { get; set; }
    }
}
