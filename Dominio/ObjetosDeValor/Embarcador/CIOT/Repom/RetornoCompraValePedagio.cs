using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("viagem")]
    public class RetornoCompraValePedagio
    {
        [XmlElement("viagem_codigo")]
        public string CodigoViagem { get; set; }

        [XmlElement("filial_codigo_cliente")]
        public string CodigoFilialCliente { get; set; }

        [XmlElement("processo_cliente_codigo")]
        public string CodigoProcessoCliente { get; set; }

        [XmlElement("valor_total_pedagios")]
        public string ValorTotalPedagios { get; set; }

        [XmlElement("data_emissao")]
        public string DataEmissao { get; set; }

        [XmlElement("pedagios")]
        public RetornoCompraValePedagioPedagios Pedagios { get; set; }
    }
}
