using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("cavalo")]
    public class RetornoContratoFreteCavalo
    {
        [XmlElement("placa")]
        public string Placa { get; set; }

        [XmlElement("modelo")]
        public string Modelo { get; set; }
    }
}
