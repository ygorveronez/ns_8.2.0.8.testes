using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("retorno_antt")]
    public class RetornoIntegracaoCadastroVeiculoANTT
    {
        [XmlArray("veiculo_erros_antt"), XmlArrayItem("veiculo_erro_antt")]
        public RetornoIntegracaoCadastroVeiculoANTTErro[] Erros { get; set; }

        [XmlElement("validado_antt")]
        public int? ValidadoANTT { get; set; }
    }
}
