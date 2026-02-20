using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
    [XmlRoot(ElementName = "brCTe", Namespace = "")]
    public sealed class BrCTe
    {
        [XmlElement(ElementName = "ALBCnpj")]
        public double ALBCnpj { get; set; }

        [XmlElement(ElementName = "CTeAddress")]
        public CTeAddress CTeAddress { get; set; }

        [XmlElement(ElementName = "CTeNumber")]
        public int CTeNumber { get; set; }

        [XmlElement(ElementName = "CTeAccessCode")]
        public string CTeAccessCode { get; set; }

        [XmlElement(ElementName = "CTeIssuanceDate")]
        public string CTeIssuanceDate { get; set; }
    }
}
