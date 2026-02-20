using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo
{
    [XmlRoot(ElementName = "Exception", Namespace = "")]
    public sealed class Exception
    {
        [XmlElement(ElementName = "ExceptionReasonCode", Namespace = "")]
        public string ExceptionReasonCode { get; set; }

        [XmlElement(ElementName = "ExceptionDescription", Namespace = "")]
        public string ExceptionDescription { get; set; }
    }
}
