using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("pedagio")]
    public class RetornoCompraValePedagioPedagio
    {
        [XmlElement("concessionaria_codigo")]
        public string ConcessionarioaCodigo { get; set; }

        [XmlElement("concessionaria_nome")]
        public string ConcessionariaNome { get; set; }

        [XmlElement("praca_codigo")]
        public string CodigoPraca { get; set; }

        [XmlElement("praca_nome")]
        public string NomePraca { get; set; }

        [XmlElement("numero_eixos")]
        public string NumeroEixos { get; set; }

        [XmlElement("valor")]
        public string Valor { get; set; }
    }
}
