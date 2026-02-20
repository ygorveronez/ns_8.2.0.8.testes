using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("retorno_antt")]
    public class RetornoIntegracaoCadastroCarretaANTT
    {
        [XmlArray("carreta_erros_antt"), XmlArrayItem("carreta_erro_antt")]
        public RetornoIntegracaoCadastroCarretaANTTErro[] Erros { get; set; }

        [XmlElement("validado_antt")]
        public int? ValidadoANTT { get; set; }
    }
}
