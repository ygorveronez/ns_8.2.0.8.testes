using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class StatusConsignacaoDadosFreteMaritimoRoteamento
    {
        [XmlElement(ElementName = "RoutingCode")]
        public string CodigoRoteamento { get; set; }

        [XmlElement(ElementName = "TypeOfShipment")]
        public string TipoRemessa { get; set; }

        [XmlElement(ElementName = "CarrierSCACCode")]
        public string CodigoSCAC { get; set; }

        [XmlElement(ElementName = "PortOfLoading")]
        public StatusConsignacaoDadosFreteMaritimoRoteamentoPorto PortoCarga { get; set; }

        [XmlElement(ElementName = "PortOfDischarge")]
        public StatusConsignacaoDadosFreteMaritimoRoteamentoPorto PortoDescarga { get; set; }

        [XmlElement(ElementName = "VoyageNo")]
        public string NumeroViagem { get; set; }

        [XmlElement(ElementName = "VesselName")]
        public string NomeNavio { get; set; }

        [XmlElement(ElementName = "VesselFlag")]
        public string FlagNavio { get; set; }

        [XmlElement(ElementName = "References")]
        public StatusConsignacaoDadosReferencia[] Referencias { get; set; }
    }
}
