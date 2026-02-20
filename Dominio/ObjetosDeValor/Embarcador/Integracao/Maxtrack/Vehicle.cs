using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Maxtrack
{
    [XmlRoot(ElementName = "vehicle")]
    public class Vehicle
    {
        [XmlElement(ElementName = "plate")]
        public string plate { get; set; }

    }
}
