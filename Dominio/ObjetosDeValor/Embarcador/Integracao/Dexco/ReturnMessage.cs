using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Dexco
{
    [XmlRoot(ElementName = "ReturnMessage")]
    public class ReturnMessage
    {
        [XmlElement(ElementName = "ReturnCreateFO")]
        public ReturnCreateFO[] ReturnCreateFO { get; set; }
    }
}
