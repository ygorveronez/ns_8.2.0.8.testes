using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("quitacao")]
    public class RetornoContratoFreteQuitacao
    {
        [XmlElement("quitacao_local")]
        public string LocalQuitacao { get; set; }

        [XmlElement("quitacao_local_descricao")]
        public string DescricaoLocalQuitacao { get; set; }

        [XmlElement("quitacao_local_tipo")]
        public string TipoLocalQuitacao { get; set; }

        [XmlElement("quitacao_data_prevista")]
        public string DataPrevistaQuitacao { get; set; }

        [XmlElement("bandeira")]
        public string Bandeira { get; set; }

        [XmlElement("endereco")]
        public string Endereco { get; set; }

        [XmlElement("cidade")]
        public string Cidade { get; set; }

        [XmlElement("estado")]
        public string Estado { get; set; }

        [XmlElement("telefone")]
        public string Telefone { get; set; }

        [XmlElement("preco_diesel")]
        public string PrecoDiesel { get; set; }

        [XmlElement("cnpj")]
        public string CNPJ { get; set; }

        [XmlElement("margem_cliente")]
        public string MargemCliente { get; set; }

        [XmlElement("carga_peso_previsto")]
        public string PesoPrevistoCarga { get; set; }

        [XmlElement("valor_tarifa_cliente")]
        public string ValorTarifaCliente { get; set; }
    }
}
