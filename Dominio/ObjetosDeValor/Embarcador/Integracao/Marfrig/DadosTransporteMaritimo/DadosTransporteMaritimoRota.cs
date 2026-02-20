using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo
{
    public sealed class DadosTransporteMaritimoRota
    {
        [JsonProperty(PropertyName = "ETATransbordo", Order = 1, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? ETATransbordo { get; set; }

        [JsonProperty(PropertyName = "ETSTransbordo", Order = 2, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? ETSTransbordo { get; set; }

        [JsonProperty(PropertyName = "codigoRota", Order = 3, Required = Required.Default)]
        public string CodigoRota { get; set; }

        [JsonProperty(PropertyName = "nomeNavioTransbordo", Order = 4, Required = Required.Default)]
        public string NomeNavioTransbordo { get; set; }

        [JsonProperty(PropertyName = "numeroViagemTransbordo", Order = 5, Required = Required.Default)]
        public string NumeroViagemTransbordo { get; set; }

        [JsonProperty(PropertyName = "portoCarregamentoTransbordo", Order = 6, Required = Required.Default)]
        public string PortoCarregamentoTransbordo { get; set; }

        [JsonProperty(PropertyName = "portoDestinoTransbordo", Order = 7, Required = Required.Default)]
        public string PortoDestinoTransbordo { get; set; }
    }
}
