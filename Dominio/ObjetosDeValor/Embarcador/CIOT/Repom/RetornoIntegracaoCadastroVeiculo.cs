using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("veiculo")]
    public class RetornoIntegracaoCadastroVeiculo
    {
        [XmlElement("retorno_antt")]
        public RetornoIntegracaoCadastroVeiculoANTT ANTT { get; set; }
    }
}
