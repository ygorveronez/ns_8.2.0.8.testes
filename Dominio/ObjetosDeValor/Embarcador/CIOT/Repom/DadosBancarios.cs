using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("dados_bancarios")]
    public class DadosBancarios
    {
        [XmlElement("banco")]
        public string Banco { get; set; }

        [XmlElement("agencia")]
        public string Agencia { get; set; }

        [XmlElement("agencia_dv")]
        public string DigitoVerificadorAgencia { get; set; }

        [XmlElement("conta_corrente")]
        public string ContaCorrente { get; set; }

        [XmlElement("conta_corrente_dv")]
        public string DigitoVerificadorContaCorrente { get; set; }

        [XmlElement("titular_conta_corrente")]
        public string Titular { get; set; }

        [XmlElement("titular_cnpj_cpf_conta_corrente")]
        public string CPFCNPJTitular { get; set; }
    }
}
