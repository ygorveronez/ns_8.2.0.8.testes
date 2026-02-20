using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class CrossTalkBody
    {
        [XmlElement(ElementName = "CrossTalk_Version_Body")]
        public CrossTalkVersionBody VersionBody { get; set; }
    }
}
