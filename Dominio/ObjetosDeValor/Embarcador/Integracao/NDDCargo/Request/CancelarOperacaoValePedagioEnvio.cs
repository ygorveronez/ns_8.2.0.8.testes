using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "cancelarOperacaoValePedagio_envio", Namespace = "http://www.nddigital.com.br/nddcargo")]
    public class CancelarOperacaoValePedagioEnvio
    {
        [XmlAttribute(AttributeName = "token")]
        public string Token { get; set; }

        [XmlAttribute(AttributeName = "versao")]
        public string Versao { get; set; }

        [XmlElement(ElementName = "infCancelarOperacaoValePedagio")]
        public InfCancelarOperacaoValePedagio InfCancelarOperacaoValePedagio { get; set; }
    }
}
