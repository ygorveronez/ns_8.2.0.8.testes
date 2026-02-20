using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Correios
{
    public class DimensaoObjeto
    {

        [XmlElement(ElementName = "tipo_objeto")]
        public string TipoObjeto { get; set; }

        [XmlElement(ElementName = "dimensao_altura")]
        public string Altura { get; set; }

        [XmlElement(ElementName = "dimensao_largura")]
        public string Largura { get; set; }

        [XmlElement(ElementName = "dimensao_comprimento")]
        public string Comprimento { get; set; }

        [XmlElement(ElementName = "dimensao_diametro")]
        public string Diametro { get; set; }

    }
}
