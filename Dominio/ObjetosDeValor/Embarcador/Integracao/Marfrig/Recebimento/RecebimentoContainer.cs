using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento
{
    public sealed class RecebimentoContainer
    {
        [JsonProperty(PropertyName = "dataDepositoContainer", Order = 1, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? DataDepositoContainer { get; set; }

        [JsonProperty(PropertyName = "dataRetiraVazio", Order = 2, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? DataRetiraVazio { get; set; }

        [JsonProperty(PropertyName = "dataRetornoVazio", Order = 3, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? DataRetornoVazio { get; set; }

        [JsonProperty(PropertyName = "numeroContainer", Order = 4, Required = Required.Default)]
        public string NumeroContainer { get; set; }

        [JsonProperty(PropertyName = "numeroLacre", Order = 5, Required = Required.Default)]
        public string NumeroLacre { get; set; }

        [JsonProperty(PropertyName = "terminalContainer", Order = 6, Required = Required.Default)]
        public string TerminalContainer { get; set; }
    }
}
