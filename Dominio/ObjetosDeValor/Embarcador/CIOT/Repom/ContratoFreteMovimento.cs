using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("movimento")]
    public class ContratoFreteMovimento
    {
        [XmlElement("movimento_codigo")]
        public string CodigoMovimento { get; set; }

        [XmlElement("movimento_codigo_cliente")]
        public string CodigoMovimentoCliente { get; set; }

        [XmlElement("valor")]
        public string Valor { get; set; }
    }
}
