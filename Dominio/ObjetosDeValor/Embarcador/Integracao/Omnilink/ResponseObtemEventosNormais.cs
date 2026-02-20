using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink
{
    [XmlRoot(ElementName = "ObtemEventosNormaisResult")]
    public class ResponseObtemEventosNormais
    {
        [XmlElement(ElementName = "TeleEvento")]
        public List<TeleEvento> TeleEventos { get; set; }
    }

}
