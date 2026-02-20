using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento
{
    public sealed class RecebimentoReserva
    {
        [JsonProperty(PropertyName = "codigoArmador", Order = 1, Required = Required.Default)]
        public string CodigoArmador { get; set; }

        [JsonProperty(PropertyName = "dataBooking", Order = 2, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? DataBooking { get; set; }

        [JsonProperty(PropertyName = "despachante", Order = 3, Required = Required.Default)]
        public string Despachante { get; set; }

        [JsonProperty(PropertyName = "nomeNavio", Order = 4, Required = Required.Default)]
        public string NomeNavio { get; set; }

        [JsonProperty(PropertyName = "numeroBL", Order = 5, Required = Required.Default)]
        public string NumeroBL { get; set; }

        [JsonProperty(PropertyName = "numeroBooking", Order = 6, Required = Required.Default)]
        public string NumeroBooking { get; set; }

        [JsonProperty(PropertyName = "numeroViagem", Order = 7, Required = Required.Default)]
        public string NumeroViagem { get; set; }

        [JsonProperty(PropertyName = "portoCarregamento", Order = 8, Required = Required.Default)]
        public string PortoCarregamento { get; set; }
    }
}
