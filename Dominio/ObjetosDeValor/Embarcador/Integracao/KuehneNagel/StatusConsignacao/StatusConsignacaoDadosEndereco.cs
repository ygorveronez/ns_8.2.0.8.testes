using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class StatusConsignacaoDadosEndereco
    {
        [XmlElement(ElementName = "AddressTypeCode")]
        public string CodigoTipoEndereco { get; set; }

        [XmlElement(ElementName = "PartyInformation")]
        public DadosParticipante Participante { get; set; }
    }
}
