using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("processo_transporte")]
    public class RetornoQuitacaoContratoFreteProcessoTransporte
    {
        [XmlElement("data")]
        public string Data { get; set; }

        [XmlElement("cliente_codigo")]
        public string CodigoCliente { get; set; }

        [XmlElement("cliente_cnpj")]
        public string CNPJCliente { get; set; }

        [XmlElement("cliente_razao_social")]
        public string RazaoSocialCliente { get; set; }

        [XmlElement("codigo_cartao")]
        public string CodigoCartao { get; set; }

        [XmlElement("processo_transporte_codigo")]
        public string CodigoProcessoTransporte { get; set; }

        [XmlElement("utiliza_codigo_processo")]
        public string UtilizaCodigoProcesso { get; set; }

        [XmlElement("codigo_processo_descricao")]
        public string DescricaoCodigoProcesso { get; set; }

        [XmlElement("processo_cliente_codigo")]
        public string CodigoProcessoCliente { get; set; }

        [XmlElement("processo_cliente_filial")]
        public string CodigoFilialRepom { get; set; }

        [XmlElement("processo_cliente_filial_codigo_cliente")]
        public string CodigoFilialCliente { get; set; }

        [XmlElement("processo_transporte_status")]
        public string StatusProcessoTransporte { get; set; }

        [XmlElement("quitacao_local")]
        public string LocalQuitacao { get; set; }

        [XmlElement("quitacao_local_tipo")]
        public string TipoLocalQuitacao { get; set; }

        [XmlElement("quitacao_local_descricao")]
        public string DescricaoLocalQuitacao { get; set; }

        [XmlElement("quitacao_modo_pagamento")]
        public string ModoPagamentoQuitacao { get; set; }

        [XmlElement("quitacao_contrato")]
        public string ContratoQuitacao { get; set; }

        [XmlElement("operacao_codigo")]
        public string CodigoOperacao { get; set; }

        [XmlElement("valor_final")]
        public string ValorFinal { get; set; }

        [XmlAttribute("baixa_antecipada")]
        public string BaixaAntecipada { get; set; }

        [XmlElement("contratado")]
        public RetornoQuitacaoContratoFreteProcessoTransporteContratado Contratado { get; set; }

        [XmlElement("movimentacao_financeira")]
        public RetornoQuitacaoContratoFreteProcessoTransporteMovimentacaoFinanceira MovimentacaoFinanceira { get; set; }

        [XmlElement("cupons_quitacao")]
        public string CuponsQuitacao { get; set; }

        [XmlElement("mensagem_motorista")]
        public string MensagemMotorista { get; set; }

        [XmlArray("documentos")]
        public RetornoQuitacaoContratoFreteProcessoTransporteDocumento[] Documentos { get; set; }

        [XmlElement("valor_inss_patronal")]
        public string ValorINSSPatronal { get; set; }

        [XmlElement("valor_pedagio")]
        public string ValorPedagio { get; set; }

        [XmlElement("base_calculo")]
        public string BaseCalculo { get; set; }

        [XmlElement("saldo_conta_contratado")]
        public string SaldoContaContratado { get; set; }

        [XmlElement("saldo_conta_combustivel")]
        public string SaldoContaCombustivel { get; set; }

        [XmlElement("saldo_atual_cartao_v3")]
        public string SaldoAtualCartaoV3 { get; set; }
    }
}
