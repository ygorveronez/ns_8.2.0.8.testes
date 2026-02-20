using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class OrdemTransporteDadosEnvio
    {
        [XmlElement(ElementName = "ModeOfTransport", Order = 1)]
        public string ModoTransporte { get; set; }

        [XmlElement(ElementName = "ETS", Order = 2)]
        public DataHora DataETS { get; set; }

        [XmlElement(ElementName = "TermsOfDelivery", Order = 3)]
        public string TermoEntrega { get; set; }

        [XmlElement(ElementName = "TermsOfDeliveryLocation", Order = 4)]
        public string TermoEntregaLocalizacao { get; set; }

        [XmlElement(ElementName = "TypeOfShipment", Order = 5)]
        public int TipoEnvio { get; set; }

        [XmlElement(ElementName = "CountryOfDestination", Order = 6)]
        public string PaisDestino { get; set; }

        [XmlElement(ElementName = "CountryOfOrigin", Order = 7)]
        public string PaisOrigem { get; set; }

        [XmlElement(ElementName = "PortOfLoading", Order = 8)]
        public string PortoOrigem { get; set; }

        [XmlElement(ElementName = "PortOfDischarge", Order = 9)]
        public string PortoDestino { get; set; }

        [XmlElement(ElementName = "References", Order = 10)]
        public OrdemTransporteDadosEnvioReferencia[] Referencias { get; set; }

        [XmlElement(ElementName = "CargoDetails", Order = 11)]
        public OrdemTransporteDadosEnvioDetalhesCarga DetalhesCarga { get; set; }
    }
}
