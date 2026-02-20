using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin
{
    public class RespostaConteudoDetalhe
    {
        [JsonProperty(PropertyName = "customerName", Required = Required.Default)]
        public string ClienteNome { get; set; }
        [JsonProperty(PropertyName = "customerCode", Required = Required.Default)]
        public string CodigoCliente { get; set; }
        [JsonProperty(PropertyName = "city", Required = Required.Default)]
        public string Cidade { get; set; }
        [JsonProperty(PropertyName = "state", Required = Required.Default)]
        public string Uf { get; set; }
        [JsonProperty(PropertyName = "chargeNumber", Required = Required.Default)]
        public string NumeroPedidoEmbarcador { get; set; }
        [JsonProperty(PropertyName = "itemDescription", Required = Required.Default)]
        public string DescricaoItem { get; set; }
        [JsonProperty(PropertyName = "salesOrder", Required = Required.Default)]
        public string NumeroPedido { get; set; }
        [JsonProperty(PropertyName = "itemCode", Required = Required.Default)]
        public string CodigoItem { get; set; }
        [JsonProperty(PropertyName = "quantityMet", Required = Required.Default)]
        public int Quantidade { get; set; }
        [JsonProperty(PropertyName = "totalWeight", Required = Required.Default)]
        public decimal Peso { get; set; }
        [JsonProperty(PropertyName = "scheduleShip", Required = Required.Default)]
        public DateTime DataFaturamento { get; set; }
        [JsonProperty(PropertyName = "productLine", Required = Required.Default)]
        public string TipoCarga { get; set; }
        [JsonProperty(PropertyName = "messageIdentifierCode", Required = Required.Default)]
        public string MessageIdentifierCode { get; set; }
        [JsonProperty(PropertyName = "fileId", Required = Required.Default)]
        public string FileId { get; set; }

    }
}
