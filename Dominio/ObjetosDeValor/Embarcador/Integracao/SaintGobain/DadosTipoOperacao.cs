using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain
{
    [XmlRoot(ElementName = "E1EDL21")]
    public sealed class DadosTipoOperacao
    {
        [XmlElement(ElementName = "LFART")]
        public string CodigoIntegracaoTipoOperacao { get; set; }
    }
}
