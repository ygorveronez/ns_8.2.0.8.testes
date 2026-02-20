using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao
{
    [XmlRoot(ElementName = "Request", Namespace = "br.com.carrefour.supply.transp.ocorrenciaCTe")]
    public class Request
    {
        [XmlElement(ElementName = "mensagemOcorrencia", Namespace = "br.com.carrefour.supply.transp.ocorrenciaCTe")]
        public string MensagemOcorrencia { get; set; }

        [XmlElement(ElementName = "nomeArquivo", Namespace = "br.com.carrefour.supply.transp.ocorrenciaCTe")]
        public string NomeArquivo { get; set; }

        [XmlElement(ElementName = "tipoArquivo", Namespace = "br.com.carrefour.supply.transp.ocorrenciaCTe")]
        public string TipoArquivo { get; set; }

        [XmlAttribute(AttributeName = "ns0", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns0 { get; set; }
    }
}
