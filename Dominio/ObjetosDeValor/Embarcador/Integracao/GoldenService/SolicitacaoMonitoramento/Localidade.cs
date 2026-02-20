using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento
{
    public class Localidade
    {
        [XmlElement(ElementName = "PAIS")]
        public string Pais { get; set; }

        [XmlElement(ElementName = "CIDADE")]
        public string Cidade { get; set; }

        [XmlElement(ElementName = "UF")]
        public string UF { get; set; }

        [XmlElement(ElementName = "RUA")]
        public string Rua { get; set; }

        [XmlElement(ElementName = "NUM")]
        public string Numero { get; set; }

        [XmlElement(ElementName = "COMP")]
        public string Complemento { get; set; }

        [XmlElement(ElementName = "BAIRRO")]
        public string Bairro { get; set; }

        [XmlElement(ElementName = "OBS")]
        public string Observacao { get; set; }
    }
}
