using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento
{

    public class Request
    {
        [XmlElement(ElementName = "LOGIN")]
        public Login Login { get; set; }

        [XmlElement(ElementName = "SERVICO")]
        public string Servico { get; set; }

        [XmlElement(ElementName = "VEICULO")]
        public Veiculo Veiculo { get; set; }

        [XmlElement(ElementName = "CARRETA1")]
        public Veiculo Carreta1 { get; set; }

        [XmlElement(ElementName = "CARRETA2")]
        public Veiculo Carreta2 { get; set; }

        [XmlElement(ElementName = "VIAGEM")]
        public Viagem Viagem { get; set; }

        [XmlElement(ElementName = "TRANSPORTADOR")]
        public Transportador Transportador { get; set; }

        [XmlElement(ElementName = "EMBARCADOR")]
        public Embarcador Embarcador { get; set; }

        [XmlElement(ElementName = "CONDUTOR1")]
        public Condutor Condutor1 { get; set; }

        [XmlElement(ElementName = "CONDUTOR2")]
        public Condutor Condutor2 { get; set; }

        [XmlElement(ElementName = "AJUDANTE")]
        public Ajudante Ajudante { get; set; }

        [XmlElement(ElementName = "CARGA")]
        public Carga Carga { get; set; }

        [XmlElement(ElementName = "ORIGEM")]
        public Localidade Origem { get; set; }

        [XmlElement(ElementName = "DESTINO")]
        public Localidade Destino { get; set; }
    }
}
