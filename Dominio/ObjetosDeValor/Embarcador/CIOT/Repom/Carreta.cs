using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("carreta")]
    public class Carreta
    {
        [XmlElement("contratado_cnpj_cpf")]
        public string CPFCNPJContratado { get; set; }

        [XmlElement("placa")]
        public string Placa { get; set; }

        [XmlElement("numero_eixos")]
        public string NumeroEixos { get; set; }

        [XmlElement("ano")]
        public string Ano { get; set; }

        [XmlElement("cor")]
        public string Cor { get; set; }

        [XmlElement("numero_chassis")]
        public string NumeroChassis { get; set; }

        [XmlElement("renavam")]
        public string RENAVAM { get; set; }

        [XmlElement("cidade")]
        public string Cidade { get; set; }

        [XmlElement("estado")]
        public string Estado { get; set; }
        
        [XmlElement("rntrc_codigo")]
        public string RNTRC { get; set; }
    }
}
