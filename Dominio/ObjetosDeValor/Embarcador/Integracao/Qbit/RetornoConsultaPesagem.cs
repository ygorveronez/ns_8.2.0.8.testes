using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Qbit
{
    [XmlRoot("ListaPesagem")]
    public class RetornoConsultaPesagem
    {
        [XmlElement("CodigoRetorno")]
        public int CodigoRetorno { get; set; }

        [XmlElement("DescricaoRetorno")]
        public string DescricaoRetorno { get; set; }

        [XmlArray("Pesagens"), XmlArrayItem("Pesagem")]
        public RetornoConsultaPesagemLista[] Pesagens { get; set; }
    }
}
