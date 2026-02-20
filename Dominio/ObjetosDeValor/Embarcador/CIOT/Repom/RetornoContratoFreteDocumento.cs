using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("documento")]
    public class RetornoContratoFreteDocumento
    {
        [XmlElement("codigo")]
        public string Codigo { get; set; }

        [XmlElement("serie")]
        public string Serie { get; set; }

        [XmlElement("filial_codigo_cliente")]
        public string CodigoMovimentoCliente { get; set; }
    }
}
