using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "retornoCancelarOperacaoValePedagio")]
    public class RetornoCancelarOperacaoValePedagio
    {
        [XmlElement(ElementName = "mensagens")]
        public Mensagens Mensagens { get; set; }

        [XmlElement(ElementName = "envioCancelarOperacaoValePedagio")]
        public EnvioCancelarOperacaoValePedagio EnvioCancelarOperacaoValePedagio { get; set; }
    }
}
