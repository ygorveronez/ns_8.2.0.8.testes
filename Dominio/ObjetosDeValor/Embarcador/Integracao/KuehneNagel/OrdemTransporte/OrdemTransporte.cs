using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    [XmlRoot(ElementName = "TransportOrderExtLight")]
    public sealed class OrdemTransporte
    {
        [XmlElement(ElementName = "Envelope", Order = 1)]
        public OrdemTransporteIdentificacao Identificacao { get; set; }

        [XmlElement(ElementName = "Message", Order = 2)]
        public OrdemTransporteDados Dados { get; set; }
    }
}
