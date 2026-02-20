using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class DadosParticipante
    {
        [XmlElement(ElementName = "PartyName")]
        public string Nome { get; set; }

        [XmlElement(ElementName = "PartyName_2")]
        public string SegundoNome { get; set; }

        [XmlElement(ElementName = "PartyNumber")]
        public string Numero { get; set; }

        [XmlElement(ElementName = "Street")]
        public string Rua { get; set; }

        [XmlElement(ElementName = "Street_2")]
        public string SegundaRua { get; set; }

        [XmlElement(ElementName = "POBox")]
        public string POBox { get; set; }

        [XmlElement(ElementName = "City")]
        public string Cidade { get; set; }

        [XmlElement(ElementName = "ZIPCode")]
        public string CodigoCompactacao { get; set; }

        [XmlElement(ElementName = "CountryCode")]
        public string CodigoPais { get; set; }
    }
}
