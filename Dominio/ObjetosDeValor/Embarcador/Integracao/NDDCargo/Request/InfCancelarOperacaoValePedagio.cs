using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "infCancelarOperacaoValePedagio")]
    public class InfCancelarOperacaoValePedagio
    {
        [XmlElement(ElementName = "cnpj")]
        public string CNPJ { get; set; }

        [XmlElement(ElementName = "autorizacao")]
        public Autorizacao Autorizacao { get; set; }

        [XmlElement(ElementName = "motivoCancelamento")]
        public string MotivoCancelamento { get; set; }
    }
}
