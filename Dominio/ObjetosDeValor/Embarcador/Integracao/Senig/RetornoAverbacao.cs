using System;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senig
{
    [XmlRoot(ElementName = "return")]
    public sealed class RetornoAverbacao
    {
        [XmlIgnore]
        public DateTime? DataRecebimento { get; set; }

        [XmlElement(ElementName = "dtRec")]
        public string DataRecebimentoXML 
        { 
            get { return DataRecebimento?.ToDateTimeString(true) ?? string.Empty; }
            set { DataRecebimento = value.ToNullableDateTime() ; } 
        }

        [XmlElement(ElementName = "cProtocolo")]
        public string Protocolo { get; set; }

        [XmlElement(ElementName = "listaMensagem")]
        public MensagemAverbacao Mensagem { get; set; }
    }
}
