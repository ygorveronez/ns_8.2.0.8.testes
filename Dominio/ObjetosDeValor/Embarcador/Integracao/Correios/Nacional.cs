using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Correios
{
    public class Nacional
    {

        [XmlElement(ElementName = "bairro_destinatario")]
        public string BairroDestinatario { get; set; }

        [XmlElement(ElementName = "cidade_destinatario")]
        public string CidadeDestinatario { get; set; }

        [XmlElement(ElementName = "uf_destinatario")]
        public string UFDestinatario { get; set; }

        [XmlElement(ElementName = "cep_destinatario")]
        public string CEPDestinatario { get; set; }

        [XmlElement(ElementName = "codigo_usuario_postal")]
        public string CodigoUsuarioPostal { get; set; }

        [XmlElement(ElementName = "centro_custo_cliente")]
        public string CentroCustoCliente { get; set; }

        [XmlElement(ElementName = "numero_nota_fiscal")]
        public string NumeroNotaFiscal { get; set; }

        [XmlElement(ElementName = "serie_nota_fiscal")]
        public string SerieNotaFiscal { get; set; }

        [XmlElement(ElementName = "valor_nota_fiscal")]
        public string ValorNotaFiscal { get; set; }

        [XmlElement(ElementName = "natureza_nota_fiscal")]
        public string NaturezaNotaFiscal { get; set; }

        [XmlElement(ElementName = "descricao_objeto")]
        public string DescricaoObjeto { get; set; }

        [XmlElement(ElementName = "valor_a_cobrar")]
        public string ValorACobrar { get; set; }

    }
}
