using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class StatusConsignacaoIdentificacao
    {
        [XmlElement(ElementName = "SenderIdentification")]
        public string IdentificacaoRemetente { get; set; }

        [XmlElement(ElementName = "ReceiverIdentification")]
        public string IdentificacaoDestinatario { get; set; }

        [XmlElement(ElementName = "MessageType")]
        public string TipoIdentificacao { get; set; }

        [XmlElement(ElementName = "MessageVersion")]
        public string Versao { get; set; }

        [XmlElement(ElementName = "MessageFunctionCode")]
        public string CodigoFuncao { get; set; }

        [XmlElement(ElementName = "EnvelopeIdentification")]
        public string Identificacao { get; set; }

        [XmlElement(ElementName = "MessageDateTime")]
        public DataHora Data { get; set; }
    }
}
