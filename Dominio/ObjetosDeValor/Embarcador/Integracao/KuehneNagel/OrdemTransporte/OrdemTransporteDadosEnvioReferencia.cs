using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class OrdemTransporteDadosEnvioReferencia
    {
        [XmlElement(ElementName = "AddressType", Order = 1)]
        public string TipoEndereco { get; set; }

        [XmlElement(ElementName = "ReferenceType", Order = 2)]
        public string TipoReferencia { get; set; }

        [XmlElement(ElementName = "Reference", Order = 3)]
        public string Referencia { get; set; }
    }
}
