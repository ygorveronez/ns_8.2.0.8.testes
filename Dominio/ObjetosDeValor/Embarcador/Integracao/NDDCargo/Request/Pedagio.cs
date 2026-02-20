using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "pedagio")]
    public class Pedagio
    {
        [XmlElement(ElementName = "valor")]
        public decimal Valor { get; set; }

        [XmlElement(ElementName = "origem")]
        public string Origem { get; set; }

        [XmlElement(ElementName = "destino")]
        public string Destino { get; set; }
    }
}
