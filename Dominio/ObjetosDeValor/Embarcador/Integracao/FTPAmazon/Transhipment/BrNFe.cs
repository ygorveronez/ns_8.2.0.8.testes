using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
    [XmlRoot(ElementName = "brNFe", Namespace = "")]
    public sealed class BrNFe
    {
        [XmlElement(ElementName = "sellerCnpj")]
        public double SellerCNPJ { get; set; }

        [XmlElement(ElementName = "customerCnpjCpf")]
        public double CustomerCNPJCPF { get; set; }

        [XmlElement(ElementName = "nfeNumber")]
        public int NFeNumber { get; set; }

        [XmlElement(ElementName = "nfeSeries")]
        public string NFeSeries { get; set; }

        [XmlElement(ElementName = "nfeAccessCode")]
        public string NFeAccessCode { get; set; }

        [XmlElement(ElementName = "nfeIssuanceDate")]
        public string NFeIssuanceDate { get; set; }

        [XmlElement(ElementName = "nfeICMSSTAmount")]
        public decimal NFeICMSSTAmount { get; set; }

        [XmlElement(ElementName = "nfeICMSAmount")]
        public decimal NFeICMSAmount { get; set; }

        [XmlElement(ElementName = "nfeTotalValue")]
        public decimal NFeTotalValue { get; set; }

        [XmlElement(ElementName = "nfeProductsTotalValue")]
        public decimal NFeProductsTotalValue { get; set; }
    }
}
