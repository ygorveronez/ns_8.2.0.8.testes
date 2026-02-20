using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("erros")]
    public class RetornoIntegracaoErro
    {
        [XmlElement("erro")]
        public RetornoIntegracaoErroErro[] Erros { get; set; }
    }
}
