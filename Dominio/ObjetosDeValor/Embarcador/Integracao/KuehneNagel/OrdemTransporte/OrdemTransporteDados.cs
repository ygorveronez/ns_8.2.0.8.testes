using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class OrdemTransporteDados
    {
        [XmlElement(ElementName = "ControllingOffice", Order = 1)]
        public string Controladoria { get; set; }

        [XmlElement(ElementName = "AddressInformation", Order = 2)]
        public OrdemTransporteDadosEndereco Endereco { get; set; }

        [XmlElement(ElementName = "ShippingInformation", Order = 3)]
        public OrdemTransporteDadosEnvio Envio { get; set; }
    }
}
