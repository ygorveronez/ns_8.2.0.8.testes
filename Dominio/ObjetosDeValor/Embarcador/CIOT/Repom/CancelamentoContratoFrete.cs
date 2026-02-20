using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("contrato")]
    public class CancelamentoContratoFrete
    {
        [XmlElement("cliente_codigo")]
        public string CodigoCliente { get; set; }

        [XmlElement("processo_transporte_codigo")]
        public string CodigoProcessoTransporte { get; set; }

        [XmlElement("processo_cliente_codigo")]
        public string CodigoProcessoCliente { get; set; }

        [XmlElement("filial_codigo_cliente")]
        public string CodigoFilialCliente { get; set; }

        [XmlElement("login")]
        public string Login { get; set; }
    }
}
