using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("documento")]
    public class CompraValePedagioDocumento
    {
        [XmlElement("documento_codigo")]
        public string Numero { get; set; }

        [XmlElement("serie")]
        public string Serie { get; set; }

        [XmlElement("filial_codigo_cliente")]
        public string CodigoFilial { get; set; }

    }
}
