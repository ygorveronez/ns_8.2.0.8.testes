using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class Ide
    {
        [XmlElement(ElementName = "cnpj")]
        public string Cnpj { get; set; }

        [XmlElement(ElementName = "numero")]
        public int Numero { get; set; }

        [XmlElement(ElementName = "serie")]
        public string Serie { get; set; }

        [XmlElement(ElementName = "ptEmissor")]
        public string PtEmissor { get; set; }
    }
}
