using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Correios
{
    public class ObjetoPostal
    {

        [XmlElement(ElementName = "numero_etiqueta")]
        public string NumeroEtiqueta { get; set; }

        [XmlElement(ElementName = "sscc")]
        public string SSCC { get; set; }

        [XmlElement(ElementName = "codigo_objeto_cliente")]
        public string CodigoObjetoCliente { get; set; }

        [XmlElement(ElementName = "codigo_servico_postagem")]
        public string CodigoServicoPostagem { get; set; }

        [XmlElement(ElementName = "cubagem")]
        public string Cubagem { get; set; }

        [XmlElement(ElementName = "peso")]
        public string Peso { get; set; }

        [XmlElement(ElementName = "rt1")]
        public string RT1 { get; set; }

        [XmlElement(ElementName = "rt2")]
        public string RT2 { get; set; }

        [XmlElement(ElementName = "restricao_anac")]
        public string RestricaoANAC { get; set; }

        [XmlElement(ElementName = "destinatario")]
        public Destinatario Destinatario { get; set; }

        [XmlElement(ElementName = "nacional")]
        public Nacional Nacional { get; set; }

        [XmlElement(ElementName = "servico_adicional")]
        public ServicoAdicional ServicoAdicional { get; set; }

        [XmlElement(ElementName = "dimensao_objeto")]
        public DimensaoObjeto DimensaoObjeto { get; set; }

        [XmlElement(ElementName = "data_captacao")]
        public string DataCaptacao { get; set; }

        [XmlElement(ElementName = "data_postagem_sara")]
        public string DataPostagemSara { get; set; }

        [XmlElement(ElementName = "status_processamento")]
        public string StatusProcessamento { get; set; }

        [XmlElement(ElementName = "numero_comprovante_postagem")]
        public string NumeroComprovantePostagem { get; set; }

        [XmlElement(ElementName = "valor_cobrado")]
        public string ValorCobrado { get; set; }

    }
}
