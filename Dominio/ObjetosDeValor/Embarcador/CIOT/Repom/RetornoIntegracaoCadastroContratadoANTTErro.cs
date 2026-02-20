using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("contratado_erro_antt")]
    public class RetornoIntegracaoCadastroContratadoANTTErro
    {
        [XmlElement("erro_codigo")]
        public string Codigo { get; set; }

        [XmlElement("erro_descricao")]
        public string Descricao { get; set; }
    }
}
