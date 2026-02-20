using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo
{
    public sealed class DadosTransporteMaritimoTransporte
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

        [JsonProperty(PropertyName = "nomeTerminalOrigem", Order = 12, Required = Required.Default)]
        public string nomeTerminalOrigem { get; set; }

        [JsonProperty(PropertyName = "numeroCarga", Order = 13, Required = Required.Always)]
        public string NumeroCarga { get; set; }

        [JsonProperty(PropertyName = "genset", Order = 14, Required = Required.Always)]
        public string Genset { get; set; }

        [JsonProperty(PropertyName = "tipoContainer", Order = 15, Required = Required.Always)]
        public string TipoContainer { get; set; }

        [JsonProperty(PropertyName = "usaProbe", Order = 16, Required = Required.Always)]
        public string UsaProbe { get; set; }

        [JsonProperty(PropertyName = "viaTransporte", Order = 17, Required = Required.Always)]
        public string ViaTransporte { get; set; }

        [JsonProperty(PropertyName = "tipoTransporte", Order = 18, Required = Required.Always)]
        public string TipoTransporte { get; set; }

        [JsonProperty(PropertyName = "cargaPallet", Order = 19, Required = Required.Default)]
        public string CargaPalletizada { get; set; }

        [JsonProperty(PropertyName = "temperatura", Order = 20, Required = Required.Default)]
        public string Temperatura { get; set; }

        [JsonProperty(PropertyName = "inland", Order = 21, Required = Required.Default)]
        public string Inland { get; set; }

        [JsonProperty(PropertyName = "segundaDeadLineCarga", Order = 22, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? SegundaDeadLineCarga { get; set; }

        [JsonProperty(PropertyName = "tipoFrete", Order = 23, Required = Required.Default)]
        public string TipoFrete { get; set; }

        [JsonProperty(PropertyName = "segundaDeadLineDraft", Order = 24, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? SegundaDeadLineDraft { get; set; }

        [JsonProperty(PropertyName = "valorCapatazia", Order = 25, Required = Required.Default)]
        public int ValorCapatazia { get; set; }

        [JsonProperty(PropertyName = "moedaCapatazia", Order = 26, Required = Required.Default)]
        public string MoedaCapatazia { get; set; }

        [JsonProperty(PropertyName = "valorFrete", Order = 27, Required = Required.Default)]
        public int ValorFrete { get; set; }

        [JsonProperty(PropertyName = "destinoTransbordo", Order = 28, Required = Required.Default)]
        public string DestinoTransbordo { get; set; }

        [JsonProperty(PropertyName = "codigoTerminalOrigem", Order = 29, Required = Required.Default)]
        public string CodigoterminalOrigem { get; set; }

    }
}
