using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class Pedido
    {
        [JsonProperty(PropertyName = "site")]
        public string CodigoIntegracaoFilial { get; set; }

        [JsonProperty(PropertyName = "invoice_number")]
        public string NumeroNotasFiscais { get; set; }

        [JsonProperty(PropertyName = "order_number")]
        public string NumeroPedido { get; set; }

        [JsonProperty(PropertyName = "service_type")]
        public string TipoServicoRoteirizacao { get; set; }

        [JsonProperty(PropertyName = "redelivery")]
        public bool Reentrega { get; set; }

        [JsonProperty(PropertyName = "customerCreationDate")]
        public string DataCriacaoPedido { get; set; }

        [JsonProperty(PropertyName = "location")]
        public LocalColetaEntrega LocalColetaEntrega { get; set; }

        [JsonProperty(PropertyName = "loads")]
        public List<decimal> DadosCarregamento { get; set; }

        [JsonProperty(PropertyName = "schedule_date")]
        public List<PedidoRestricaoAgendamento> RestricoesAgendamento { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public PedidoMetadados Metadados { get; set; }

        [JsonProperty(PropertyName = "items")]
        public List<PedidoItem> Itens { get; set; }

        [JsonProperty(PropertyName = "additional_info")]
        public List<string> InformacoesAdicionais { get; set; }

        [JsonProperty(PropertyName = "expectedDeliveryDate")]
        public string DataPrevisaoEntrega { get; set; }

        [JsonProperty(PropertyName = "expectedLoadingDate")]
        public string DataColetaPedido { get; set; }

        [JsonProperty(PropertyName = "constraints")]
        public PedidoRestricao Restricao { get; set; }
    }
}
