using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class PontosParada
    {
        [XmlElement(ElementName = "pontoParada")]
        public List<PontoParada> PontoParada { get; set; }
    }
}
