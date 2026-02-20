using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "ndvp")]
    public class AutorizacaoNDVP
    {
        [XmlElement(ElementName = "numero")]
        public long Numero { get; set; }
        
        [XmlElement(ElementName = "ndvpCodVerificador")]
        public long CodigoVerificador { get; set; }
    }
}
