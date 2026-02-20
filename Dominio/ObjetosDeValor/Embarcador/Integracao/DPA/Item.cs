using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DPA
{
    public class Item
    {
        [JsonProperty("item")]
        public int QuantidadeDocumento { get; set; }

        [JsonProperty("ciotNr")]
        public string NumeroCIOT { get; set; }

        [JsonProperty("cidadeOrigem")]
        public string CidadeOrigem { get; set; }

        [JsonProperty("estadoOrigem")]
        public string EstadoOrigem { get; set; }

        [JsonProperty("cidadeDestino")]
        public string CidadeDestino { get; set; }

        [JsonProperty("estadoDestino")]
        public string EstadoDestino { get; set; }

        [JsonProperty("nfeNumber")]
        public string NumeroNFE { get; set; }

        [JsonProperty("pedidoFrete")]
        public string PedidoFrete { get; set; }

        [JsonProperty("moeda")]
        public string Moeda { get; set; }

        [JsonProperty("valorFrete")]
        public decimal ValorFrete { get; set; }

    }
}
