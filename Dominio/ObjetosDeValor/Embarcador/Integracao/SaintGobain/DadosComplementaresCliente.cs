using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain
{
    [XmlRoot(ElementName = "ZE1ADRM1WE")]
    public sealed class DadosComplementaresCliente
    {
        [XmlElement(ElementName = "SMTP_ADDR")]
        public string EmailPrincipal { get; set; }
    }
}
