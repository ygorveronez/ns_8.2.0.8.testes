using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("passagem")]
    public  class RetornoContratoFretePassagem
    {
        [XmlElement("posto_codigo")]
        public string CodigoPosto { get; set; }

        [XmlElement("data_prevista")]
        public string DataPrevista { get; set; }

        [XmlElement("nome_fantasia")]
        public string NomeFantasia { get; set; }

        [XmlElement("bandeira")]
        public string Bandeira { get; set; }

        [XmlElement("endereco")]
        public string Endereco { get; set; }

        [XmlElement("cidade")]
        public string Cidade { get; set; }

        [XmlElement("estado")]
        public string Estado { get; set; }

        [XmlElement("telefone")]
        public string Telefone { get; set; }

        [XmlElement("preco_diesel")]
        public string PrecoDiesel { get; set; }

        [XmlElement("posto_cnpj")]
        public string CNPJPosto { get; set; }
    }
}
