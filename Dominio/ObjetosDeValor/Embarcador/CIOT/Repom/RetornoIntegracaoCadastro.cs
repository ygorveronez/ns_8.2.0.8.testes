using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("integra_dados_cadastro_nacional")]
    public class RetornoIntegracaoCadastro
    {
        [XmlElement("contratado")]
        public RetornoIntegracaoCadastroContratado Contratado { get; set; }

        [XmlElement("veiculo")]
        public RetornoIntegracaoCadastroVeiculo Veiculo { get; set; }

        [XmlElement("carreta")]
        public RetornoIntegracaoCadastroCarreta Carreta { get; set; }
    }
}
