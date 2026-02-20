using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain
{
    [XmlRoot(ElementName = "DELVRY03")]
    public sealed class Delivery
    {
        [XmlElement(ElementName = "IDOC")]
        public Begin Dados { get; set; }
    }
}
