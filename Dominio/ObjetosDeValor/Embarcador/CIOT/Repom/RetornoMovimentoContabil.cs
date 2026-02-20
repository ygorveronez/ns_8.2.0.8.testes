using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("Movimento")]
    public class RetornoMovimentoContabil
    {
        [XmlElement("Contabil")]
        public string Contabil { get; set; }

        [XmlElement("Registros")]
        public string Registros { get; set; }
    }
}
