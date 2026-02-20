using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("carreta_erro_antt")]
    public class RetornoIntegracaoCadastroCarretaANTTErro
    {
        [XmlElement("erro_codigo")]
        public string Codigo { get; set; }

        [XmlElement("erro_descricao")]
        public string Descricao { get; set; }
    }
}
