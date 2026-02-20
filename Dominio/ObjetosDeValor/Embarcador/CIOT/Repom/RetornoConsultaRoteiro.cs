using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("roteiros")]
    public class RetornoConsultaRoteiro
    {
        [XmlElement("roteiro")]
        public RetornoConsultaRoteiroRoteiro[] Roteiros { get; set; }
    }
}
