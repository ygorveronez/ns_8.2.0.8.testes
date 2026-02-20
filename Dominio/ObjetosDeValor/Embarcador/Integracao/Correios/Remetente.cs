using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Correios
{
    public class Remetente
    {

        [XmlElement(ElementName = "numero_contrato")]
        public long NumeroContrato { get; set; }

        [XmlElement(ElementName = "numero_diretoria")]
        public int NumeroDiretoria { get; set; }

        [XmlElement(ElementName = "codigo_administrativo")]
        public int CodigoAdministrativo { get; set; }

        [XmlElement(ElementName = "nome_remetente")]
        public string Nome { get; set; }

        [XmlElement(ElementName = "logradouro_remetente")]
        public string Logradouro { get; set; }

        [XmlElement(ElementName = "numero_remetente")]
        public string Numero { get; set; }

        [XmlElement(ElementName = "complemento_remetente")]
        public string Complemento { get; set; }

        [XmlElement(ElementName = "bairro_remetente")]
        public string Bairro { get; set; }

        [XmlElement(ElementName = "cep_remetente")]
        public string CEP { get; set; }

        [XmlElement(ElementName = "cidade_remetente")]
        public string Cidade { get; set; }

        [XmlElement(ElementName = "uf_remetente")]
        public string UF { get; set; }

        [XmlElement(ElementName = "telefone_remetente")]
        public string Telefone { get; set; }

        [XmlElement(ElementName = "fax_remetente")]
        public string Fax { get; set; }

        [XmlElement(ElementName = "email_remetente")]
        public string Email { get; set; }

        [XmlElement(ElementName = "celular_remetente")]
        public string Celular { get; set; }

        [XmlElement(ElementName = "cpf_cnpj_remetente")]
        public string CPF_CNPJ { get; set; }

        [XmlElement(ElementName = "ciencia_conteudo_proibido")]
        public string CienciaConteudoProibido { get; set; }

    }
}
