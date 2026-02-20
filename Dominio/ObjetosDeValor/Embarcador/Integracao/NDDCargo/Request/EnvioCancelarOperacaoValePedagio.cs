using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "envioCancelarOperacaoValePedagio")]
    public class EnvioCancelarOperacaoValePedagio
    {
        [XmlAttribute(AttributeName = "token")]
        public string Token { get; set; }

        [XmlElement(ElementName = "cnpj")]
        public string CNPJ { get; set; }

        [XmlElement(ElementName = "autorizacao")]
        public Autorizacao Autorizacao { get; set; }

        [XmlElement(ElementName = "motivoCancelamento")]
        public string MotivoCancelamento { get; set; }
    }
}
