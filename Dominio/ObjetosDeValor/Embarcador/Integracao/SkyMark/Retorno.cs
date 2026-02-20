using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark
{
    [XmlRoot("WsAtualizacaoPerfilAutonomo", Namespace = "http://ws.skymark.net.br/")]
    public class RetornoIntegracaoAtualizacaoPerfilAutonomo
    {
        [XmlElement("resultado")]
        public bool Resultado { get; set; }

        [XmlElement("mensagem")]
        public string Mensagem { get; set; }

        [XmlElement("numero")]
        public string Numero { get; set; }
    }
}