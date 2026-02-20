using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("processo_transporte")]
    public class RetornoContratoFrete
    {
        [XmlElement("os_codigo")]
        public string CodigoOS { get; set; }

        [XmlElement("os_tipo")]
        public string TipoOS { get; set; }

        [XmlElement("processo_transporte_codigo")]
        public string CodigoProcessoTransporte { get; set; }

        [XmlElement("processo_cliente_codigo")]
        public string CodigoProcessoCliente { get; set; }

        [XmlElement("processo_cliente_filial_codigo_cliente")]
        public string CodigoFilial { get; set; }

        [XmlElement("filial_descricao")]
        public string DescricaoFilial { get; set; }

        [XmlElement("data_emissao")]
        public string DataEmissao { get; set; }

        [XmlElement("agenda_codigo")]
        public string CodigoAgenda { get; set; }

        [XmlElement("contratado")]
        public RetornoContratoFreteContratado Contratado { get; set; }

        [XmlElement("motorista")]
        public RetornoContratoFreteMotorista Motorista { get; set; }

        [XmlElement("cavalo")]
        public RetornoContratoFreteCavalo Cavalo { get; set; }

        [XmlElement("carreta")]
        public RetornoContratoFreteCarreta Carreta { get; set; }

        [XmlElement("dados_viagem")]
        public RetornoContratoFreteDadosViagem DadosViagem { get; set; }

        [XmlArray("movimentos"), XmlArrayItem("movimento")]
        public RetornoContratoFreteMovimento[] Movimentos { get; set; }

        [XmlArray("documentos"), XmlArrayItem("documento")]
        public RetornoContratoFreteDocumento[] Documentos { get; set; }

        [XmlArray("passagens"), XmlArrayItem("passagem")]
        public RetornoContratoFretePassagem[] Passagens { get; set; }

        [XmlElement("quitacao")]
        public RetornoContratoFreteQuitacao Quitacao { get; set; }

        [XmlArray("itens_quitacao"), XmlArrayItem("item_quitacao")]
        public RetornoContratoFreteItemQuitacao[] ItensQuitacao { get; set; }

        [XmlElement("observacoes_contrato")]
        public string ObservacoesContrato { get; set; }

        [XmlElement("observacoes_termos")]
        public string ObservacoesTermos { get; set; }
    }
}
