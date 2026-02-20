using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain
{
    [XmlRoot(ElementName = "IDOC")]
    public sealed class Begin
    {
        [XmlElement(ElementName = "E1EDL20")]
        public DadosCarga DadosCarga { get; set; }
        
        [XmlElement(ElementName = "EDI_DC40")]
        public DadosComplementares DadosComplementares { get; set; }
    }
}
