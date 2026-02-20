using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senig
{
    [XmlRoot(ElementName = "listaMensagem")]
    public sealed class MensagemAverbacao
    {

        [XmlIgnore]
        public CodigoRetorno Codigo { get; set; }

        [XmlElement(ElementName = "cStatus")]
        public string Status { get; set; }

        [XmlElement(ElementName = "dStatus")]
        public int CodigoXML
        {
            get { return (int)Codigo; }
            set { Codigo = (CodigoRetorno)value; }
        }

        [XmlElement(ElementName = "dDetErr")]
        public string Detalhes { get; set; }
    }
}
