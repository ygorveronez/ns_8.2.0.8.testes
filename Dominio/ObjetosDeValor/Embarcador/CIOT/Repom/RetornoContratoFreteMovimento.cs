using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("movimento")]
    public class RetornoContratoFreteMovimento
    {
        [XmlElement("movimento_codigo_cliente")]
        public string CodigoMovimentoCliente { get; set; }

        [XmlElement("movimento_descricao")]
        public string DescricaoMovimento { get; set; }
        
        [XmlElement("valor")]
        public string Valor { get; set; }

        [XmlElement("tipo")]
        public string Tipo { get; set; }
    }
}
