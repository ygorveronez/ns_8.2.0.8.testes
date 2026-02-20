using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("contratado")]
    public class Contratado
    {
        [XmlElement("contratado_cnpj_cpf")]
        public string CPFCNPJ { get; set; }

        /// <summary>
        /// 0 = PF
        /// 1 = PJ optante pelo Simples
        /// 2- PJ Não optante pelo Simples
        /// </summary>
        [XmlElement("pessoa_tipo")]
        public string Tipo { get; set; }

        [XmlElement("nome_contratado")]
        public string RazaoSocial { get; set; }

        [XmlElement("nome_fantasia")]
        public string NomeFantasia { get; set; }

        [XmlElement("cep")]
        public string CEP { get; set; }

        [XmlElement("endereco")]
        public string Endereco { get; set; }

        [XmlElement("bairro")]
        public string Bairro { get; set; }

        [XmlElement("cidade")]
        public string Cidade { get; set; }

        [XmlElement("estado")]
        public string Estado { get; set; }

        [XmlElement("telefone1")]
        public string Telefone1 { get; set; }

        [XmlElement("telefone2")]
        public string Telefone2 { get; set; }

        [XmlElement("celular")]
        public string Celular { get; set; }

        [XmlElement("email")]
        public string Email { get; set; }

        [XmlElement("contato")]
        public string Contato { get; set; }

        [XmlElement("dependentes")]
        public string Dependentes { get; set; }

        [XmlElement("inss_codigo")]
        public string CodigoINSS { get; set; }

        /// <summary>
        /// 0 - Não
        /// 1 - Sim
        /// </summary>
        [XmlElement("inss_simplificado")]
        public string INSSSimplificado { get; set; }

        [XmlElement("rntrc_codigo")]
        public string RNTRC { get; set; }

        [XmlElement("rntrc_data_emissao")]
        public string DataEmissaoRNTRC { get; set; }

        [XmlElement("rntrc_data_vencimento")]
        public string DataVencimentoRNTRC { get; set; }

        [XmlElement("dados_bancarios")]
        public DadosBancarios DadosBancarios { get; set; }
    }
}
