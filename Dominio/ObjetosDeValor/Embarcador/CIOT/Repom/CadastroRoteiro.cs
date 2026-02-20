using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("solicita_roteiros")]
    public class CadastroRoteiro
    {
        [XmlElement("roteiro")]
        public CadastroRoteiroRoteiro Roteiro { get; set; }
    }
}
