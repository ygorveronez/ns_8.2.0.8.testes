using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    [XmlRoot(ElementName = "Validacao")]
    public sealed class OrdemTransporteValidacao
    {
        [XmlElement(ElementName = "Erro")]
        public string[] MensagensErro { get; set; }
    }
}
