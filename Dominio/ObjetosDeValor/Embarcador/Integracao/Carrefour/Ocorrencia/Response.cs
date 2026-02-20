using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Ocorrencia
{
    [XmlRoot(ElementName = "Response", Namespace = "br.com.carrefour.supply.transp.complementarCTe")]
    public class Response
    {
        [XmlElement(ElementName = "resultadoOperacao", Namespace = "br.com.carrefour.supply.transp.complementarCTe")]
        public string ResultadoOperacao { get; set; }

        [XmlElement(ElementName = "mensagemErro", Namespace = "br.com.carrefour.supply.transp.complementarCTe")]
        public string MensagemErro { get; set; }

        [XmlAttribute(AttributeName = "ns0", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns0 { get; set; }
    }
}
