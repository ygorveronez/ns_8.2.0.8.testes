using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "retornoOperacaoValePedagio")]
    public class RetornoOperacaoValePedagio
    {
        [XmlElement(ElementName = "mensagens")]
        public Mensagens Mensagens { get; set; }

        [XmlElement(ElementName = "retOperacaoValePedagio")]
        public RetOperacaoValePedagio RetOperacaoValePedagio { get; set; }
    }
}
