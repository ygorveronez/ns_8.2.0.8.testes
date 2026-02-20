using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Qbit
{
    [XmlRoot("Retorno")]
    public class RetornoConsultaPesagemErro
    {
        [XmlElement("CodigoRetorno")]
        public int CodigoRetorno { get; set; }

        [XmlElement("DescricaoRetorno")]
        public string DescricaoRetorno { get; set; }
    }
}