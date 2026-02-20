using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("motorista")]
    public class RetornoContratoFreteMotorista
    {
        [XmlElement("motorista_cpf")]
        public string CPF { get; set; }

        [XmlElement("motorista_nome")]
        public string Nome { get; set; }

        [XmlElement("carteira_habilitacao")]
        public string CNH { get; set; }
    }
}