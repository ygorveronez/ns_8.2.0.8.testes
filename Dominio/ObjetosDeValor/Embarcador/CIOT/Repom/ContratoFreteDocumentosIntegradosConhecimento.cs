using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("conhecimento")]
    public class ContratoFreteDocumentosIntegradosConhecimento
    {
        [XmlElement("ctrc_codigo")]
        public string Numero { get; set; }

        [XmlElement("ctrc_serie")]
        public string Serie { get; set; }

        [XmlElement("ctrc_filial_codigo_cliente")]
        public string CodigoFilialEmissora { get; set; }

        [XmlElement("documento_tipo")]
        public string TipoDocumento { get; set; }

        [XmlArray("nfs"), XmlArrayItem("nf")]
        public ContratoFreteDocumentosIntegradosNotaFiscal[] NotasFiscais { get; set; }
    }
}
