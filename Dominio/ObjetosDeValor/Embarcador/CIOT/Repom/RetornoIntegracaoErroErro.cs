using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("erro")]
    public class RetornoIntegracaoErroErro
    {
        [XmlElement("erro_codigo")]
        public string Codigo { get; set; }

        [XmlElement("erro_descricao")]
        public string Descricao { get; set; }
    }
}
