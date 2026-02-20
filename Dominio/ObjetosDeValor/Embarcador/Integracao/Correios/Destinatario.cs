using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Correios
{
    public class Destinatario
    {
        [XmlElement(ElementName = "nome_destinatario")]
        public string Nome { get; set; }

        [XmlElement(ElementName = "telefone_destinatario")]
        public string Telefone { get; set; }

        [XmlElement(ElementName = "celular_destinatario")]
        public string Celular { get; set; }

        [XmlElement(ElementName = "email_destinatario")]
        public string Email { get; set; }

        [XmlElement(ElementName = "logradouro_destinatario")]
        public string Logradouro { get; set; }

        [XmlElement(ElementName = "complemento_destinatario")]
        public string Complemento { get; set; }

        [XmlElement(ElementName = "numero_end_destinatario")]
        public string Numero { get; set; }

        [XmlElement(ElementName = "cpf_cnpj_destinatario")]
        public string CPFCNPJ { get; set; }

    }
}
