using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class StatusConsignacaoDadosStatus
    {
        [XmlElement(ElementName = "StatusCode")]
        public string Codigo { get; set; }

        [XmlElement(ElementName = "StatusDateTime")]
        public DataHora Data { get; set; }

        [XmlElement(ElementName = "StatusLocation")]
        public string Localizacao { get; set; }

        [XmlElement(ElementName = "StatusCreationDateTime")]
        public DataHora DataCriacao { get; set; }

        [XmlElement(ElementName = "StatusRemarks")]
        public string Observacao { get; set; }

        [XmlElement(ElementName = "HistoricalStatus")]
        public StatusConsignacaoDadosStatus[] Historicos { get; set; }
    }
}
