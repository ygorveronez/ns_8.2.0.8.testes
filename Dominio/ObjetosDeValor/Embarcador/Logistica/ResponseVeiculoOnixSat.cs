using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    [XmlRoot(ElementName = "Veiculo")]
    public class VeiculoOnixat
    {
        [XmlElement(ElementName = "veiID")]
        public string VeiID { get; set; }
        [XmlElement(ElementName = "placa")]
        public string Placa { get; set; }
    }


    [XmlRoot(ElementName = "ResponseVeiculo")]
    public class ResponseVeiculoOnixsat
    {
        [XmlElement(ElementName = "Veiculo")]
        public List<VeiculoOnixat> Veiculo { get; set; }
    }
}
