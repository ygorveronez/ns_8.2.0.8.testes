using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("recibo_quitacao")]
    public class RetornoQuitacaoContratoFrete
    {
        [XmlElement("data")]
        public string Data { get; set; }

        [XmlElement("hora")]
        public string Hora { get; set; }

        [XmlElement("mensagem")]
        public string Mensagem { get; set; }

        [XmlElement("telefone_repom")]
        public string Telefone { get; set; }

        [XmlElement("condicao_fiscal")]
        public string CondicaoFiscal { get; set; }

        [XmlElement("sistema_contas")]
        public string SistemaContas { get; set; }

        [XmlElement("processo_transporte")]
        public RetornoQuitacaoContratoFreteProcessoTransporte ProcessoTransporte { get; set; }
    }
}
