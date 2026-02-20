using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("contratado")]
    public class RetornoQuitacaoContratoFreteProcessoTransporteContratado
    {
        [XmlElement("contratado_cnpj_cpf")]
        public string CPFCNPJContratado { get; set; }

        [XmlElement("nome_contratado")]
        public string NomeContratado { get; set; }

        [XmlElement("rg")]
        public string RG { get; set; }

        [XmlElement("fone")]
        public string Telefone { get; set; }

        [XmlElement("celular")]
        public string Celular { get; set; }

        [XmlElement("tipo")]
        public string Tipo { get; set; }

        [XmlElement("razao_social")]
        public string RazaoSocial { get; set; }

        [XmlElement("nome_fantasia")]
        public string NomeFantasia { get; set; }

        [XmlElement("utiliza_dados_bancarios")]
        public string UtilizaDadosBancarios { get; set; }

        [XmlElement("dados_bancarios")]
        public RetornoQuitacaoContratoFreteProcessoTransporteContratadoDadosBancarios DadosBancarios { get; set; }
    }
}
