using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Correios
{
    public class PLP
    {

        [XmlElement(ElementName = "id_plp")]
        public string id_plp { get; set; }

        [XmlElement(ElementName = "valor_global")]
        public string ValorGlobal { get; set; }

        [XmlElement(ElementName = "mcu_unidade_postagem")]
        public string MCUUnidadePostagem { get; set; }

        [XmlElement(ElementName = "nome_unidade_postagem")]
        public string NomeUnidadePostagem { get; set; }

        [XmlElement(ElementName = "cartao_postagem")]
        public string CartaoPostagem { get; set; }

    }
}
