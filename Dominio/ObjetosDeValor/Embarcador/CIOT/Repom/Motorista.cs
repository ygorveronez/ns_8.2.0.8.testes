using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("motorista")]
    public class Motorista
    {
        [XmlElement("cpf")]
        public string CPF { get; set; }

        [XmlElement("contratado_cnpj_cpf")]
        public string CPFCNPJContratado { get; set; }

        [XmlElement("nome")]
        public string Nome { get; set; }

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

        [XmlElement("telefone")]
        public string Telefone { get; set; }

        [XmlElement("celular")]
        public string Celular { get; set; }

        [XmlElement("email")]
        public string Email { get; set; }

        [XmlElement("naturalidade")]
        public string Naturalidade { get; set; }

        [XmlElement("naturalidade_estado")]
        public string EstadoNaturalidade { get; set; }

        [XmlElement("data_nascimento")]
        public string DataNascimento { get; set; }

        [XmlElement("nome_pai")]
        public string NomePai { get; set; }

        [XmlElement("nome_mae")]
        public string NomeMae { get; set; }

        [XmlElement("rg")]
        public string RG { get; set; }

        [XmlElement("rg_data_emissao")]
        public string DataEmissaoRG { get; set; }

        [XmlElement("rg_orgao_emissor")]
        public string OrgaoEmissorRG { get; set; }

        [XmlElement("rg_estado_emissor")]
        public string EstadoEmissorRG { get; set; }

        [XmlElement("carteira_habilitacao")]
        public string CNH { get; set; }

        [XmlElement("carteira_habilitacao_data_emissao")]
        public string DataEmissaoCNH { get; set; }

        [XmlElement("carteira_habilitacao_categoria")]
        public string CategoriaCNH { get; set; }

        [XmlElement("carteira_habilitacao_data_validade")]
        public string DataValidadeCNH { get; set; }

        [XmlElement("carteira_habilitacao_data_habilitacao")]
        public string DataPrimeiraCNH { get; set; }
    }
}
