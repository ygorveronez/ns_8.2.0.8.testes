using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao
{
    [XmlRoot(ElementName = "Response", Namespace = "br.com.carrefour.supply.transp.ocorrenciaCTe")]
    public class Response
    {
        [XmlElement(ElementName = "resultadoOperacao", Namespace = "br.com.carrefour.supply.transp.ocorrenciaCTe")]
        public string ResultadoOperacao { get; set; }

        [XmlElement(ElementName = "mensagemErro", Namespace = "br.com.carrefour.supply.transp.ocorrenciaCTe")]
        public string MensagemErro { get; set; }

        [XmlAttribute(AttributeName = "ns0", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns0 { get; set; }
    }
}
