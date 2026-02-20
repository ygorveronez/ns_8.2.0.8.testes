using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class PontoParada
    {
        [XmlElement(ElementName = "codigoIBGE")]
        public int CodigoIBGE { get; set; }
    }
}
