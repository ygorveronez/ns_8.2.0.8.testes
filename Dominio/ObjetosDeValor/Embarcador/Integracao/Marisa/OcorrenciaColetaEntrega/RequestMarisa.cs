using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa
{
    public class RequestMarisa
    {
        [JsonProperty("external_order_numbers")]
        public List<RequestMarisaNumeroPedidosExternos> RequestMarisaNumeroPedidosExternos;

        [JsonProperty("volume_number")]
        public string NumeroVolumes;

        [JsonProperty("tracking_code")]
        public string NumeroRastreio;

        [JsonProperty("sales_order_number")]
        public string NumeroPedidoVenda;

        [JsonProperty("invoice")]
        public List<RequestMarisaNotaFiscal> RequestMarisaNotaFiscais;

        [JsonProperty("estimated_delivery_date")]
        public string DataEntregaEstimada;

        [JsonProperty("order_number")]
        public string NumeroPedido;

        [JsonProperty("tracking_url")]
        public string URLRastreioPedido;

        [JsonProperty("history")]
        public RequestMarisaHistorico RequestMarisaHistorico;
    }
}