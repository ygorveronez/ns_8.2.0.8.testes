using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class OrdemTransporteDadosEndereco
    {
        [XmlElement(ElementName = "Customer", Order = 1)]
        public DadosParticipante Cliente { get; set; }

        [XmlElement(ElementName = "Shipper", Order = 2)]
        public OrdemTransporteDadosEnderecoRemetente Remetente { get; set; }

        [XmlElement(ElementName = "Consignee", Order = 3)]
        public OrdemTransporteDadosEnderecoConsignatario Consignatario { get; set; }
    }
}
