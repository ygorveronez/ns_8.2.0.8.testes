using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain
{
    [XmlRoot(ElementName = "E1ADRM1")]
    public sealed class DadosCliente
    {
        [XmlElement(ElementName = "PARTNER_Q")]
        public string TipoCliente { get; set; }

        [XmlElement(ElementName = "PARTNER_ID")]
        public string Codigo { get; set; }

        [XmlElement(ElementName = "NAME1")]
        public string Nome { get; set; }

        [XmlElement(ElementName = "STREET1")]
        public string Endereco { get; set; }

        [XmlElement(ElementName = "COUNTRY1")]
        public string Pais { get; set; }

        [XmlElement(ElementName = "CITY1")]
        public string Cidade { get; set; }

        [XmlElement(ElementName = "REGION")]
        public string Estado { get; set; }

        [XmlElement(ElementName = "TELEPHONE1")]
        public string Telefone { get; set; }

        [XmlElement(ElementName = "TELEPHONE2")]
        public string TelefoneSecundario { get; set; }

        [XmlElement(ElementName = "E_MAIL")]
        public string Email { get; set; }

        [XmlElement(ElementName = "ZE1ADRM1WE")]
        public DadosComplementaresCliente DadosComplementaresCliente { get; set; }
    }
}
