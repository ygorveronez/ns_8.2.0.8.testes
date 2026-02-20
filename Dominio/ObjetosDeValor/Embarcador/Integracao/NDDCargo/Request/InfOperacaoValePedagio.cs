using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class InfOperacaoValePedagio
    {
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }

        [XmlAttribute(AttributeName = "impAuto")]
        public int ImpAuto { get; set; }

        [XmlAttribute(AttributeName = "tipoPagamento")]
        public int TipoPagamento { get; set; }

        [XmlElement(ElementName = "cnpj")]
        public string Cnpj { get; set; }

        [XmlElement(ElementName = "ide")]
        public Ide Ide { get; set; }

        [XmlElement(ElementName = "transportador")]
        public Transportador Transportador { get; set; }

        [XmlElement(ElementName = "condutorFavorecido")]
        public CondutorFavorecido CondutorFavorecido { get; set; }

        [XmlElement(ElementName = "infRota")]
        public InfRota InfRota { get; set; }

        [XmlElement(ElementName = "veiculo")]
        public Veiculo Veiculo { get; set; }

        [XmlElement(ElementName = "informacoesTag")]
        public InformacoesTag InformacoesTag { get; set; }
    }
}
