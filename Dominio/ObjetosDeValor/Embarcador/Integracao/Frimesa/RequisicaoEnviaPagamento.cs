using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa
{
    public class RequisicaoEnviaPagamento
    {
        [JsonProperty(PropertyName = "msPagamentoId")]
        public int MSPagamentoID { get; set; }

        [JsonProperty(PropertyName = "numeroCarga")]
        public string NumeroCarga { get; set; }

        [JsonProperty(PropertyName = "organizationCode")]
        public string OrganizationCode { get; set; }

        [JsonProperty(PropertyName = "cnpjTransportador")]
        public string CNPJTransportador { get; set; }

        [JsonProperty(PropertyName = "placaVeiculo")]
        public string PlacaVeiculo { get; set; }

        [JsonProperty(PropertyName = "placaReboque")]
        public string PlacaReboque { get; set; }

        [JsonProperty(PropertyName = "dataServico")]
        public string DataServico { get; set; }

        [JsonProperty(PropertyName = "cnpjOrigem")]
        public string CNPJOrigem { get; set; }

        [JsonProperty(PropertyName = "tipoOperacao")]
        public string TipoOperacao { get; set; }

        [JsonProperty(PropertyName = "tipoCusto")]
        public string TipoCusto { get; set; }

        [JsonProperty(PropertyName = "custoAcessorio")]
        public string CustoAcessorio { get; set; }

        [JsonProperty(PropertyName = "moeda")]
        public string Moeda { get; set; }

        [JsonProperty(PropertyName = "tipoTaxa")]
        public string TipoTaxa { get; set; }

        [JsonProperty(PropertyName = "dataTaxa")]
        public string DataTaxa { get; set; }

        [JsonProperty(PropertyName = "taxa")]
        public decimal Taxa { get; set; }

        [JsonProperty(PropertyName = "pagamentoLinhas")]
        public PagamentoLinhas[] PagamentoLinhas { get; set; }
    }
}
