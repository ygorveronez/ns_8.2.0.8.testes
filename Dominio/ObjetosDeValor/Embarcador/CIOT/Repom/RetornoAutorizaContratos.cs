using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("autoriza_contratos")]
    public class RetornoAutorizaContratos
    {
        [XmlElement("autoriza_contrato")]
        public RetornoAutorizaContrato[] AutorizaContratos { get; set; }
    }
}
