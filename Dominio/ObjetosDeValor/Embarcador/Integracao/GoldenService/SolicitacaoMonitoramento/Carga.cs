using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento
{
    [XmlRoot(ElementName = "CARGA")]
    public class Carga
    {
        [XmlElement(ElementName = "OPE")]
        public string TipoOperacao { get; set; }

        [XmlElement(ElementName = "VLR")]
        public string ValorMercadoria { get; set; }

        [XmlElement(ElementName = "DES")]
        public string Descricao { get; set; }
    }
}
