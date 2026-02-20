using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class Veiculo
    {
        [XmlElement(ElementName = "placa")]
        public string Placa { get; set; }
    }
}
