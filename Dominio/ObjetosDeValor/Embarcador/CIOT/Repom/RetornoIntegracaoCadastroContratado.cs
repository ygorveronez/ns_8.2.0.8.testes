using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("contratado")]
    public class RetornoIntegracaoCadastroContratado
    {
        [XmlElement("retorno_antt")]
        public RetornoIntegracaoCadastroContratadoANTT ANTT { get; set; }
    }
}
