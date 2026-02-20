using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("documento")]
    public class RetornoQuitacaoContratoFreteProcessoTransporteDocumento
    {
        [XmlElement("documento_codigo")]
        public string Codigo { get; set; }

        [XmlElement("serie")]
        public string Serie { get; set; }

        [XmlElement("filial_codigo")]
        public string CodigoFilialRepom { get; set; }

        [XmlElement("filial_codigo_cliente")]
        public string CodigoFilialCliente { get; set; }

        [XmlElement("data_efetiva")]
        public string DataEfetiva { get; set; }
    }
}
