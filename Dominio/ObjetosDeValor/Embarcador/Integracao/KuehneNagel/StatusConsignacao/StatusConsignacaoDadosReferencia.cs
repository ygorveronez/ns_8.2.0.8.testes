using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class StatusConsignacaoDadosReferencia
    {
        [XmlElement(ElementName = "AddressTypeCode")]
        public string CodigoTipoEndereco { get; set; }

        [XmlElement(ElementName = "ReferenceCode")]
        public string CodigoReferencia { get; set; }

        [XmlElement(ElementName = "Reference")]
        public string Referencia { get; set; }
    }
}
