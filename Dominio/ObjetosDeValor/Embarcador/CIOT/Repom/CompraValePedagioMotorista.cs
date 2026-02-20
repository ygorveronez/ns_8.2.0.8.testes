using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("motorista")]
    public class CompraValePedagioMotorista
    {
        [XmlElement("motorista_cpf")]
        public string CPF { get; set; }

        [XmlElement("motorista_nome")]
        public string Nome { get; set; }

        [XmlElement("motorista_rg")]
        public string RG { get; set; }

        [XmlElement("motorista_telefone")]
        public string Telefone { get; set; }
    }
}
