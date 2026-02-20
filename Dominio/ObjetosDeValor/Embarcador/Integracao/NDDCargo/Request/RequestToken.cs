using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class RequestToken
    {
        [XmlAttribute(AttributeName = "token")]
        public string Token { get; set; }

        [XmlAttribute(AttributeName = "versao")]
        public string Versao { get; set; }
    }
}
