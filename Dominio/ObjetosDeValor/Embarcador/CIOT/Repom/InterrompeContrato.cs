using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("interrompe_contrato")]
    public class InterrompeContrato
    {
        [XmlElement("processo_transporte_codigo")]
        public string CodigoProcessoTransporte { get; set; }

        [XmlElement("processo_cliente_codigo")]
        public string CodigoProcessoCliente { get; set; }

        [XmlElement("processo_cliente_filial_codigo_cliente")]
        public string CodigoFilialCliente { get; set; }

        [XmlElement("motivo")]
        public string Motivo { get; set; }

        [XmlElement("observacao")]
        public string Observacao { get; set; }
    }
}
