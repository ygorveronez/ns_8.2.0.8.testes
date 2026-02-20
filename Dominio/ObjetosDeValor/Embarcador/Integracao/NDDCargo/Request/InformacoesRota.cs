using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class InformacoesRota
    {
        [XmlElement(ElementName = "nome")]
        public string Nome { get; set; }

        [XmlElement(ElementName = "pontosParada")]
        public PontosParada PontosParada { get; set; }
    }
}
