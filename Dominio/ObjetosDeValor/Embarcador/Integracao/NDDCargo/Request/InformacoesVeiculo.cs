using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class InformacoesVeiculo
    {
        [XmlElement(ElementName = "modelo")]
        public string Modelo { get; set; }

        [XmlElement(ElementName = "tipo")]
        public int Tipo { get; set; }

        [XmlElement(ElementName = "kmLitroVeiculo")]
        public decimal KmLitroVeiculo { get; set; }

        [XmlElement(ElementName = "RNTRCTransportador")]
        public string RNTRCTransportador { get; set; }
    }
}
