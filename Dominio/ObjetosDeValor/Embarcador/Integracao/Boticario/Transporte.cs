using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario
{
    public sealed class Transporte
    {
        [JsonProperty(PropertyName = "custoPedagio", Order = 1, Required = Required.Always)]
        [JsonConverter(typeof(Utilidades.Json.DecimalConverter), new object[] { "f2" })]
        public decimal CustoPedagio { get; set; }

        [JsonProperty(PropertyName = "custoFrete", Order = 2, Required = Required.Always)]
        [JsonConverter(typeof(Utilidades.Json.DecimalConverter), new object[] { "f2" })]
        public decimal ValorFrete { get; set; }
    }
}