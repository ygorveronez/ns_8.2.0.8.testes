using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("carreta")]
    public class RetornoContratoFreteCarreta
    {
        [XmlElement("placa")]
        public string Placa { get; set; }
    }
}
