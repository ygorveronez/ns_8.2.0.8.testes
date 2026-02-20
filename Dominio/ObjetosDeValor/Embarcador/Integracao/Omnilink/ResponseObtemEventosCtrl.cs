using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink
{

    [XmlRoot(ElementName = "ObtemEventosCtrlResult")]
    public class ResponseObtemEventosCtrl
    {
        [XmlElement(ElementName = "TeleEvento")]
        public List<TeleEvento> TeleEventos { get; set; }
    }

}
