using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento
{
    public sealed class RecebimentoTransporte
    {
        [JsonProperty(PropertyName = "ETADest", Order = 1, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? ETADestino { get; set; }

        [JsonProperty(PropertyName = "ETADest2", Order = 2, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? ETASegundoDestino { get; set; }

        [JsonProperty(PropertyName = "ETADestFinal", Order = 3, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? ETADestinoFinal { get; set; }

        [JsonProperty(PropertyName = "ETAOrigem", Order = 4, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? ETAOrigem { get; set; }

        [JsonProperty(PropertyName = "ETAOrigem2", Order = 5, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? ETASegundaOrigem { get; set; }

        [JsonProperty(PropertyName = "ETAOrigemFinal", Order = 6, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? ETAOrigemFinal { get; set; }

        [JsonProperty(PropertyName = "ETS", Order = 7, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? ETS { get; set; }

        [JsonProperty(PropertyName = "dataEstufagem", Order = 8, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? DataEstufagem { get; set; }

        [JsonProperty(PropertyName = "deadLineCarga", Order = 9, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? DeadLineCarga { get; set; }

        [JsonProperty(PropertyName = "deadLineDraft", Order = 10, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? DeadLineDraft { get; set; }

        [JsonProperty(PropertyName = "retiraContainerDest", Order = 11, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? RetiraContainerDestino { get; set; }

        [JsonProperty(PropertyName = "terminalOrigem", Order = 12, Required = Required.Default)]
        public string TerminalOrigem { get; set; }
    }
}
