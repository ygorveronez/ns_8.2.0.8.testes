using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain
{
    [XmlRoot(ElementName = "E1EDL18")]
    public sealed class DadosInformacaoDeletar
    {
        [XmlElement(ElementName = "QUALF")]
        public string Tag { get; set; }
    }
}
