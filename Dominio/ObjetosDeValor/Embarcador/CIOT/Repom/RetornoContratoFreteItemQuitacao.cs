using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("item_quitacao")]
    public class RetornoContratoFreteItemQuitacao
    {
        [XmlElement("item_descricao")]
        public string DescricaoItem { get; set; }
    }
}
