using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    [XmlRoot(ElementName = "ConsignmentStatus")]
    public sealed class StatusConsignacao
    {
        [XmlElement(ElementName = "Envelope")]
        public StatusConsignacaoIdentificacao Identificacao { get; set; }

        [XmlElement(ElementName = "Message")]
        public StatusConsignacaoDados Dados { get; set; }
    }
}
