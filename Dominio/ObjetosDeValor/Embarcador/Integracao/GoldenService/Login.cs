using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService
{
    [XmlRoot(ElementName = "LOGIN")]
    public class Login
    {
        [XmlElement(ElementName = "ID")]
        public string Id { get; set; }

        [XmlElement(ElementName = "CODIGO")]
        public string Codigo { get; set; }

        [XmlElement(ElementName = "SENHA")]
        public string Senha { get; set; }
    }
}
