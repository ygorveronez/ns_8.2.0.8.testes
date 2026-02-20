using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "operacaoValePedagio_envio", Namespace = "http://www.nddigital.com.br/nddcargo")]
    public class OperacaoValePedagioEnvio : Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request.RequestToken
    {
        [XmlElement(ElementName = "infOperacaoValePedagio")]
        public InfOperacaoValePedagio InfOperacaoValePedagio { get; set; }
    }
}
