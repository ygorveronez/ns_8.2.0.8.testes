using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("via")]
    public class CadastroRoteiroRoteiroVia
    {
        [XmlElement("via_descricao")]
        public string Descricao { get; set; }
    }
}
