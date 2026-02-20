using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Bradesco
{
    [XmlRoot(ElementName = "Averbacao")]
    public sealed class RetornoAverbacaoBradesco
    {
        [XmlElement(ElementName = "Request")]
        public RequestAverbacaoBradesco Request { get; set; }

        [XmlElement(ElementName = "Response")]
        public ResponseAverbacaoBradesco Response { get; set; }

        public bool PossuiErro => Response?.Documento?.Tipo == "ERR" || !string.IsNullOrEmpty(Response?.Documento?.Erro);
        
        public DocAverbacaoBradesco Documento => Response?.Documento;
        
        public string Erro => Response?.Documento?.Erro;
    }

    public sealed class RequestAverbacaoBradesco
    {
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "Sequence")]
        public string Sequence { get; set; }

        [XmlElement(ElementName = "DateTime")]
        public string DateTime { get; set; }

        [XmlElement(ElementName = "Application")]
        public string Application { get; set; }

        [XmlElement(ElementName = "Server")]
        public string Server { get; set; }

        [XmlElement(ElementName = "Token")]
        public string Token { get; set; }
    }

    public sealed class ResponseAverbacaoBradesco
    {
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "Seguradora")]
        public SeguradoraAverbacaoBradesco Seguradora { get; set; }

        [XmlElement(ElementName = "Doc")]
        public DocAverbacaoBradesco Documento { get; set; }
    }

    public sealed class SeguradoraAverbacaoBradesco
    {
        [XmlElement(ElementName = "CNPJ")]
        public string CNPJ { get; set; }

        [XmlElement(ElementName = "Nome")]
        public string Nome { get; set; }
    }

    public sealed class DocAverbacaoBradesco
    {
        [XmlAttribute(AttributeName = "Arquivo")]
        public string NomeArquivo { get; set; }

        [XmlElement(ElementName = "Tipo")]
        public string Tipo { get; set; }

        [XmlElement(ElementName = "Chave")]
        public string Chave { get; set; }

        [XmlElement(ElementName = "Averbacao")]
        public string Averbacao { get; set; }

        [XmlElement(ElementName = "Protocolo")]
        public string Protocolo { get; set; }

        [XmlElement(ElementName = "Erro")]
        public string Erro { get; set; }
    }
}