using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("pedagios")]
    public class RetornoCompraValePedagioPedagios
    {
        [XmlElement("pedagio")]
        public RetornoCompraValePedagioPedagio[] Pedagio { get; set; }
    }
}
