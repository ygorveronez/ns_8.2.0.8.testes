using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte
{
    [XmlRoot(ElementName = "RetornoIntegracaoTelhaNorte")]
    public sealed class RetornoIntegracaoTelhaNorte
    {
        [XmlElement(ElementName = "Erro")]
        public string MensagemErro { get; set; }
    }
}
