using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("carreta")]
    public class RetornoIntegracaoCadastroCarreta
    {
        [XmlElement("retorno_antt")]
        public RetornoIntegracaoCadastroCarretaANTT ANTT { get; set; }
    }
}
