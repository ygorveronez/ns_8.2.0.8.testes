using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class StatusConsignacaoDadosFreteMaritimo
    {
        [XmlElement(ElementName = "BALTYP")]
        public string Tipo { get; set; }

        [XmlElement(ElementName = "PortOfLoading")]
        public string PortoCarga { get; set; }

        [XmlElement(ElementName = "PortOfDischarge")]
        public string PortoDescarga { get; set; }

        [XmlElement(ElementName = "PortOfLoadingETS")]
        public DataHora PortoCargaETS { get; set; }

        [XmlElement(ElementName = "PortOfDischargeETA")]
        public DataHora PortoDescargaETA { get; set; }

        [XmlElement(ElementName = "CountryOfDestination")]
        public string PaisDestino { get; set; }

        [XmlElement(ElementName = "CountryOfOrigin")]
        public string PaisOrigem { get; set; }

        [XmlElement(ElementName = "ShippingLineSCACCode")]
        public string CodigoArmador { get; set; }

        [XmlElement(ElementName = "PlaceOfReceipt")]
        public string LocalRecepcao { get; set; }

        [XmlElement(ElementName = "PlaceOfDelivery")]
        public string LocalEntrega { get; set; }

        [XmlElement(ElementName = "OceanBLNo.")]
        public string NumeroBL { get; set; }

        [XmlElement(ElementName = "VoyageNo")]
        public string NumeroViagem { get; set; }

        [XmlElement(ElementName = "VesselName")]
        public string NomeNavio { get; set; }

        [XmlElement(ElementName = "VesselFlag")]
        public string FlagNavio { get; set; }

        [XmlElement(ElementName = "ShipmentMovement")]
        public string MovimentoRemessa { get; set; }

        [XmlElement(ElementName = "FreightPayment")]
        public string PagamentoFrete { get; set; }

        [XmlElement(ElementName = "RoutingInformation")]
        public StatusConsignacaoDadosFreteMaritimoRoteamento[] Roteamentos { get; set; }
    }
}
