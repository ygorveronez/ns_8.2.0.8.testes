using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class CrossTalkVersionBody
    {
        [XmlAttribute(AttributeName = "versao")]
        public string Versao { get; set; }
    }
}
