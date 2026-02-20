using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain
{
    [XmlRoot(ElementName = "EDI_DC40")]
    public sealed class DadosComplementares
    {
        [XmlElement(ElementName = "MESTYP")]
        public string TipoAcao { get; set; }
        
        [XmlElement(ElementName = "IDOCTYP")]
        public string TipoDeCarga { get; set; } //Campo Provisório...
    }
}
