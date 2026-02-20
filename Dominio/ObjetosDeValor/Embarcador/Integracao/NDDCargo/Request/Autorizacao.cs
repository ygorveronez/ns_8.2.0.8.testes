using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "autorizacao")]
    public class Autorizacao
    {
        [XmlElement(ElementName = "cnpj")]
        public string CNPJ { get; set; }

        [XmlElement(ElementName = "ndvp")]
        public AutorizacaoNDVP Ndvp { get; set; }
    }
}
