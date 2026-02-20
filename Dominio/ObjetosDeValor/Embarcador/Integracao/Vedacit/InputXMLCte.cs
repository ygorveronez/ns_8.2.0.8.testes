using System.Xml;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit
{
    [XmlRoot("RFE")]
    public class InputXMLCte
    {
        [XmlElement(ElementName = "XML_SEFAZ")]
        public string XMLSefaz { get; set; }

        [XmlElement(ElementName = "STATUS_RFE")]
        public int StatusRFe { get; set; }

        [XmlElement(ElementName = "STATUS_RFE_DESCRIPTION")]
        public string DescricaoRFe { get; set; }

        [XmlElement(ElementName = "OBSERVACAO")]
        public string Observacao { get; set; }

        [XmlElement(ElementName = "USUARIO")]
        public string Usuario { get; set; }

        [XmlElement(ElementName = "ULTIMO_DOC_FATURA")]
        public string UltimoDocumentoFatura { get; set; }

        [XmlElement(ElementName = "FATURA_NDD_FRETE")]
        public string FaturaFreteNDD { get; set; }

        [XmlElement(ElementName = "DESCONTO_NDD")]
        public decimal DescontoNDD { get; set; }

        [XmlElement(ElementName = "DATA_FATURA_NDD")]
        public string DataFaturaNDD { get; set; }

        [XmlElement(ElementName = "DATA_VENCIMENTO_NDD")]
        public string DataVencimentoNND { get; set; }

        [XmlElement(ElementName = "RFE_ORDENS_COMPRA")]
        public string RfeOrdensCompra { get; set; }
    }
}
