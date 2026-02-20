using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("autoriza_contrato")]
    public class AutorizaContrato
    {
        [XmlElement("dias")]
        public string Dias { get; set; }

        [XmlElement("usuario")]
        public string Usuario { get; set; }

        [XmlElement("contrato_codigo")]
        public string CodigoContrato { get; set; }

        [XmlElement("processo_transporte_codigo_cliente")]
        public string CodigoProcessoTransporteCliente { get; set; }

        [XmlElement("processo_cliente_filial_codigo_cliente")]
        public string CodigoFilial { get; set; }
    }
}
