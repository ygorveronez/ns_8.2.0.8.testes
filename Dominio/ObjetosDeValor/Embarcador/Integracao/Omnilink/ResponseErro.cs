using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink
{

    [XmlRoot(ElementName = "teleevento")]
    public class ResponseErro
    {

        [XmlElement(ElementName = "codmsg")]
        public int CodMsg { get; set; }

        [XmlElement(ElementName = "msgerro")]
        public string MsgErro { get; set; }

    }

}
