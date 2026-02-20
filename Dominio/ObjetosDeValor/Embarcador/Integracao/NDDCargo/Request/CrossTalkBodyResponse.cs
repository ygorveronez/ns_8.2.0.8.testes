using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "CrossTalk_Body")]
    public class CrossTalkBodyResponse
    {
        [XmlElement(ElementName = "Body")]
        public Body Body { get; set; }

        [XmlElement(ElementName = "retornoOperacaoValePedagio")]
        public RetornoOperacaoValePedagio RetornoOperacaoValePedagio { get; set; }

        [XmlElement(ElementName = "retornoCancelarOperacaoValePedagio")]
        public RetornoCancelarOperacaoValePedagio RetornoCancelarOperacaoValePedagio { get; set; }
    }
}
