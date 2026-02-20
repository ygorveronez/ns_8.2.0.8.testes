using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class ContratoTransporte
    {
        [JsonProperty(PropertyName = "cnpj_filial", Required = Required.Default)]
        public string CNPJFilial { get; set; }

        [JsonProperty(PropertyName = "tipo_pagamento", Required = Required.Default)]
        public int TipoPagamento { get; set; }

        [JsonProperty(PropertyName = "serie", Required = Required.Default)]
        public string Serie { get; set; }

        [JsonProperty(PropertyName = "num_formulario", Required = Required.Default)]
        public string NumFormulario { get; set; }

        [JsonProperty(PropertyName = "numero_crt_sistema_externo", Required = Required.Default)]
        public string NumeroCRTSistemaExterno { get; set; }

        [JsonProperty(PropertyName = "data_emissao", Required = Required.Default)]
        public string DataEmissao { get; set; }

        [JsonProperty(PropertyName = "remetente_cnpj_cpf", Required = Required.Default)]
        public string RemetenteCnpjCpf { get; set; }

        [JsonProperty(PropertyName = "destinatario_cnpj_cpf", Required = Required.Default)]
        public string DestinatarioCnpjCpf { get; set; }

        [JsonProperty(PropertyName = "peso", Required = Required.Default)]
        public string Peso { get; set; }

        [JsonProperty(PropertyName = "volume", Required = Required.Default)]
        public string Volume { get; set; }

        [JsonProperty(PropertyName = "m3", Required = Required.Default)]
        public string M3 { get; set; }

        [JsonProperty(PropertyName = "valor_unitario", Required = Required.Default)]
        public string ValorUnitario { get; set; }

        [JsonProperty(PropertyName = "valor_frete_bruto", Required = Required.Default)]
        public string ValorFreteBruto { get; set; }

        [JsonProperty(PropertyName = "valor_frete_liquido", Required = Required.Default)]
        public string ValorFreteLiquido { get; set; }

        [JsonProperty(PropertyName = "motorista_cpf", Required = Required.Default)]
        public string MotoristaCPF { get; set; }

        [JsonProperty(PropertyName = "placa_controle", Required = Required.Default)]
        public string PlacaControle { get; set; }

        [JsonProperty(PropertyName = "placa_referencia", Required = Required.Default)]
        public string PlacaReferencia { get; set; }

        [JsonProperty(PropertyName = "valor_item_adiantamento", Required = Required.Default)]
        public string ValorItemAdiantamento { get; set; }

        [JsonProperty(PropertyName = "valor_pedagio", Required = Required.Default)]
        public string ValorPedagio { get; set; }

        [JsonProperty(PropertyName = "centro_custo_gerencial", Required = Required.Default)]
        public string CentroCustoGerencial { get; set; }

        [JsonProperty(PropertyName = "observacao", Required = Required.Default)]
        public string Observacao { get; set; }

        [JsonProperty(PropertyName = "total_acrescimos", Required = Required.Default)]
        public string TotalAcrescimos { get; set; }

        [JsonProperty(PropertyName = "total_descontos", Required = Required.Default)]
        public string TotalDescontos { get; set; }

        [JsonProperty(PropertyName = "numero_documento", Required = Required.Default)]
        public string NumeroDocumento { get; set; }

        [JsonProperty(PropertyName = "nome_cliente", Required = Required.Default)]
        public string NomeCliente { get; set; }

        [JsonProperty(PropertyName = "cnpj_cliente", Required = Required.Default)]
        public string CNPJCliente { get; set; }

        [JsonProperty(PropertyName = "centro_custo_original", Required = Required.Default)]
        public string CentroCustoOriginal { get; set; }

        [JsonProperty(PropertyName = "gerar_cpg", Required = Required.Default)]
        public int GerarCPG { get; set; }

        [JsonProperty(PropertyName = "codigo_carga_embarcador", Required = Required.Default)]
        public string CodigoCargaEmbarcador { get; set; }

        [JsonProperty(PropertyName = "data_vencimento_adiantamento", Required = Required.Default)]
        public string VencimentoAdiantamento;

        [JsonProperty(PropertyName = "data_vencimento_saldo", Required = Required.Default)]
        public string VencimentoSaldo;

        [JsonProperty(PropertyName = "centro_custo", Required = Required.Default)]
        public string CentroCusto;

        [JsonProperty(PropertyName = "tipo_compra_vale_pedagio", Required = Required.Default)]
        public string TipoCompraValePedagio;

        [JsonProperty(PropertyName = "tipo_pagamento_motorista", Required = Required.Default)]
        public string TipoPagamentoMotorista { get; set; }

        [JsonProperty(PropertyName = "base_inss", Required = Required.Default)]
        public string BaseInss { get; set; }

        [JsonProperty(PropertyName = "valor_inss", Required = Required.Default)]
        public string ValorInss { get; set; }

        [JsonProperty(PropertyName = "base_sest", Required = Required.Default)]
        public string BaseSest { get; set; }

        [JsonProperty(PropertyName = "valor_sest", Required = Required.Default)]
        public string ValorSest { get; set; }

        [JsonProperty(PropertyName = "base_senat", Required = Required.Default)]
        public string BaseSenat { get; set; }

        [JsonProperty(PropertyName = "valor_senat", Required = Required.Default)]
        public string ValorSenat { get; set; }

        [JsonProperty(PropertyName = "base_irrf", Required = Required.Default)]
        public string BaseIrrf { get; set; }

        [JsonProperty(PropertyName = "valor_irrf", Required = Required.Default)]
        public string ValorIrrf { get; set; }

        [JsonProperty(PropertyName = "valor_abastecimento", Required = Required.Default)]
        public string ValorAbastecimento { get; set; }

        [JsonProperty(PropertyName = "repomcontractcode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string RepomContractCode { get; set; }
    }
}