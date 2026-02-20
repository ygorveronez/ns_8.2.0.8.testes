using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("contratado")]
    public class RetornoContratoFreteContratado
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

        [XmlElement("contratado_nome")]
        public string RazaoSocial { get; set; }
        
        [XmlElement("cep")]
        public string CEP { get; set; }

        [XmlElement("endereco")]
        public string Endereco { get; set; }
        
        [XmlElement("estado")]
        public string Estado { get; set; }
    }
}
