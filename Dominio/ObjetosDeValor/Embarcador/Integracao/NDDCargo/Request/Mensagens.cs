using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "mensagens")]
    public class Mensagens
    {
        [XmlElement(ElementName = "mensagem")]
        public List<Mensagem> ListaMensagens { get; set; }
    }

}
