using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento
{
    [XmlRoot(ElementName = "VIAGEM")]
    public class Viagem
    {
        [XmlElement(ElementName = "DATAINI")]
        public string DataInicio { get; set; }

        [XmlElement(ElementName = "HORAINI")]
        public string HoraInicio { get; set; }

        [XmlElement(ElementName = "DATAFIM")]
        public string DataFim { get; set; }

        [XmlElement(ElementName = "HORAFIM")]
        public string HoraFim { get; set; }

    }
}
