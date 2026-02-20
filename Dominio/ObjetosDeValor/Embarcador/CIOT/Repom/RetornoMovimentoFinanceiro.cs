using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("movimento")]
    public class RetornoMovimentoFinanceiro
    {
        [XmlElement("financeiro")]
        public string Financeiro { get; set; }

        [XmlElement("registros")]
        public string Registros { get; set; }
    }
}
