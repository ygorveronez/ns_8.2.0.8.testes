using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class StatusConsignacaoDadosDetalhesContainer
    {
        [XmlElement(ElementName = "ContainerNumber")]
        public string NumeroContainer { get; set; }

        [XmlElement(ElementName = "ContainerType")]
        public string TipoContainer { get; set; }

        [XmlElement(ElementName = "SealNumber")]
        public string NumeroLacre { get; set; }

        [XmlElement(ElementName = "ContainerMovement")]
        public string MovimentoContainer { get; set; }

        [XmlElement(ElementName = "References")]
        public StatusConsignacaoDadosReferencia[] Referencias { get; set; }
    }
}
