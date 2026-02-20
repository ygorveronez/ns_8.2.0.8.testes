using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class OrdemTransporteIdentificacao
    {
        [XmlElement(ElementName = "SenderIdentification", Order = 1)]
        public string IdentificacaoRemetente { get; set; }

        [XmlElement(ElementName = "ReceiverIdentification", Order = 2)]
        public string IdentificacaoDestinatario { get; set; }

        [XmlElement(ElementName = "MessageTypes", Order = 3)]
        public string TipoIdentificacao { get; set; }

        [XmlElement(ElementName = "MessageVersion", Order = 4)]
        public string Versao { get; set; }

        [XmlElement(ElementName = "EnvelopeIdentification", Order = 5)]
        public int Identificacao { get; set; }

        [XmlElement(ElementName = "TransmissionDateTime", Order = 6)]
        public DataHora DataTransmissao { get; set; }
    }
}
