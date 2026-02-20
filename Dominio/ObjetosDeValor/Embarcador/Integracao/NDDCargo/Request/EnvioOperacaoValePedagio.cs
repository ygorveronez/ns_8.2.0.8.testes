using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "retOperacaoValePedagio")]
    public class RetOperacaoValePedagio
    {
        [XmlElement(ElementName = "ndvp")]
        public AutorizacaoNDVP AutorizacaoNDVP { get; set; }

        [XmlElement(ElementName = "pedagio")]
        public Pedagio Pedagio { get; set; }
    }
}
