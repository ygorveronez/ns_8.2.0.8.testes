using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "mensagem")]
    public class Mensagem
    {
        [XmlElement(ElementName = "categoria")]
        public string Categoria { get; set; }

        [XmlElement(ElementName = "codigo")]
        public int Codigo { get; set; }

        [XmlElement(ElementName = "mensagem")]
        public string TextoMensagem { get; set; }

        [XmlElement(ElementName = "observacao")]
        public string Observacao { get; set; }
    }
}
