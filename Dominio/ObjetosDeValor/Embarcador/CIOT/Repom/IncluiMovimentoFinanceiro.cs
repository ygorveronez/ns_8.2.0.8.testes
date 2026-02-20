using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("movimento")]
    public class IncluiMovimentoFinanceiro
    {
        [XmlElement("cliente_codigo")]
        public string CodigoCliente { get; set; }

        [XmlElement("processo_transporte_codigo")]
        public string CodigoProcessoTransporte { get; set; }

        [XmlElement("movimento_codigo")]
        public string CodigoMovimento { get; set; }

        [XmlElement("movimento_cliente_codigo")]
        public string CodigoMovimentoCliente { get; set; }

        [XmlElement("valor")]
        public string Valor { get; set; }

        [XmlElement("tipo_operacao")]
        public string TipoOperacao { get; set; }
    }
}
