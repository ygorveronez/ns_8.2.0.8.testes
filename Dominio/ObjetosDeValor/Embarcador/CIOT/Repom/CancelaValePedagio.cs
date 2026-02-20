using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("processo_transporte")]
    public class CancelaValePedagio
    {
        [XmlElement("processo_transporte_codigo")]
        public string CodigoProcessoTransporte { get; set; }

        [XmlElement("processo_cliente_codigo")]
        public string CodigoProcessoCliente { get; set; }

        [XmlElement("processo_cliente_filial_codigo_cliente")]
        public string CodigoProcessoClienteFilial { get; set; }

        [XmlElement("login")]
        public string Login { get; set; }
    }
}
