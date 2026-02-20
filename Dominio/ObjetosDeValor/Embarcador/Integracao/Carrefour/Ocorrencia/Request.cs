using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Ocorrencia
{
    [XmlRoot(ElementName = "Request", Namespace = "br.com.carrefour.supply.transp.complementarCTe")]
    public class Request
    {
        [XmlElement(ElementName = "chaveCTe", Namespace = "br.com.carrefour.supply.transp.complementarCTe")]
        public string ChaveCTe { get; set; }

        [XmlElement(ElementName = "xmlCTe", Namespace = "br.com.carrefour.supply.transp.complementarCTe")]
        public string XmlCTe { get; set; }

        [XmlElement(ElementName = "cnpjOrigem", Namespace = "br.com.carrefour.supply.transp.complementarCTe")]
        public string CnpjOrigem { get; set; }

        [XmlElement(ElementName = "cnpjDestino", Namespace = "br.com.carrefour.supply.transp.complementarCTe")]
        public string CnpjDestino { get; set; }

        [XmlAttribute(AttributeName = "ns0", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns0 { get; set; }
    }
}
