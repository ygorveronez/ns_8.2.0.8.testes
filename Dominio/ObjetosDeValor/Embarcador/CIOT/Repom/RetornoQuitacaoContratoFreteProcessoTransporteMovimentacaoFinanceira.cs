using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("movimentacao_financeira")]
    public class RetornoQuitacaoContratoFreteProcessoTransporteMovimentacaoFinanceira
    {
        [XmlElement("condicao_fiscal")]
        public string CondicaoFiscal { get; set; }

        [XmlElement("valor_frete")]
        public string ValorFrete { get; set; }

        [XmlElement("valor_adiantamento")]
        public string ValorAdiantamento { get; set; }

        [XmlElement("total_consumo")]
        public string TotalConsumo { get; set; }

        [XmlElement("total_saque")]
        public string TotalSaque { get; set; }

        [XmlElement("valor_final")]
        public string ValorFinal { get; set; }

        [XmlArray("movimentacoes")]
        public RetornoQuitacaoContratoFreteProcessoTransporteMovimentacaoFinanceiraMovimentacao[] Movimentacoes { get; set; }

        [XmlElement("total_movimentacao")]
        public string TotalMovimentacao { get; set; }
    }
}
