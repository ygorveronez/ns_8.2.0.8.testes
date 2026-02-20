using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.BBC
{
    public class RetornoDados
    {
        [JsonProperty(PropertyName = "sucesso", Required = Required.Default)]
        public bool Sucesso { get; set; }

        [JsonProperty(PropertyName = "mensagem", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "viagemId", Required = Required.Default)]
        public int CodigoViagem { get; set; }

        [JsonProperty(PropertyName = "pagamentoEventoId", Required = Required.Default)]
        public int CodigoEventoPagamento { get; set; }

        [JsonProperty(PropertyName = "pagamento", Required = Required.Default)]
        public RetornoIntegrarPagamentoViagemPagamento Pagamento { get; set; }
    }

    public class RetornoIntegrarPagamentoViagemPagamento
    {
        public int? pagamentoEventoId { get; set; }
        public decimal? valorParcela { get; set; }
        public decimal? valorMotorista { get; set; }
        public int? statusPagamento { get; set; }
        public int? formaPagamento { get; set; }
        public string mensagem { get; set; }
    }
}
