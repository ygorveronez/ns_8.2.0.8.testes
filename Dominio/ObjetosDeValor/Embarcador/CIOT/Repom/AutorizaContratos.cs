using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("autoriza_contratos")]
    public class AutorizaContratos
    {
        [XmlElement("autoriza_contrato")]
        public AutorizaContrato AutorizaContrato { get; set; }
    }
}
