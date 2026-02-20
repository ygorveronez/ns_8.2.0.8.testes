using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class StatusConsignacaoDados
    {
        [XmlElement(ElementName = "KNReference")]
        public string ReferenciaKuehneNagel { get; set; }

        [XmlElement(ElementName = "CommunicationReference")]
        public string ReferenciaComunicacao { get; set; }

        [XmlElement(ElementName = "ModeOfTransport")]
        public string ModoTransporte { get; set; }

        [XmlElement(ElementName = "TypeOfTransport")]
        public string TipoTransporte { get; set; }

        [XmlElement(ElementName = "Status")]
        public StatusConsignacaoDadosStatus Status { get; set; }

        [XmlElement(ElementName = "AddressInformation")]
        public StatusConsignacaoDadosEndereco[] Enderecos { get; set; }

        [XmlElement(ElementName = "References")]
        public StatusConsignacaoDadosReferencia[] Referencias { get; set; }

        [XmlElement(ElementName = "Seafreight")]
        public StatusConsignacaoDadosFreteMaritimo FreteMaritimo { get; set; }

        [XmlElement(ElementName = "CargoDetails")]
        public StatusConsignacaoDadosDetalhesCarga DetalhesCarga { get; set; }
    }
}
