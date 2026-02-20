using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("processo_transporte")]
    public class ContratoFrete
    {
        [XmlElement("dados_operacionais")]
        public ContratoFreteDadosOperacionais DadosOperacionais { get; set; }

        [XmlElement("configuracoes_viagem")]
        public ContratoFreteConfiguracoesViagem ConfiguracoesViagem { get; set; }

        [XmlElement("documentos_integrados")]
        public ContratoFreteDocumentosIntegrados DocumentosIntegrados { get; set; }

        [XmlElement("dados_contratado")]
        public ContratoFreteDadosContratado DadosContratado { get; set; }

        [XmlElement("dados_carga")]
        public ContratoFreteDadosCarga DadosCarga { get; set; }

        [XmlElement("dados_frete")]
        public ContratoFreteDadosFrete DadosFrete { get; set; }

        [XmlArray("movimentos")]
        [XmlArrayItem("movimento")]
        public ContratoFreteMovimento[] Movimentos { get; set; }

        [XmlElement("integracao_emissor_documentos")]
        public ContratoFreteEmissorDocumentos EmissorDocumento { get; set; }
    }
}
