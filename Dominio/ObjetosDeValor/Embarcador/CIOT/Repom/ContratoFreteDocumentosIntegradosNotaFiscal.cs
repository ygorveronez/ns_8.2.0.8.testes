using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("nf")]
    public class ContratoFreteDocumentosIntegradosNotaFiscal
    {
        [XmlElement("nf_codigo")]
        public string Numero { get; set; }

        [XmlElement("nf_serie")]
        public string Serie { get; set; }

        [XmlElement("nf_remetente_cnpj")]
        public string CNPJRemetente { get; set; }

        [XmlElement("nf_remetente_razao")]
        public string NomeRemetente { get; set; }

        [XmlElement("nf_destinatario_cnpj")]
        public string CNPJDestinatario { get; set; }

        [XmlElement("nf_destinatario_razao")]
        public string NomeDestinatario { get; set; }
    }
}
