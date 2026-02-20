using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class StatusConsignacaoDadosDetalhesCargaConteudo
    {
        [XmlElement(ElementName = "CargoDescriptionLine")]
        public string Descricao { get; set; }

        [XmlElement(ElementName = "MarksAndNumbers")]
        public string MarcasENumeros { get; set; }
    }
}
