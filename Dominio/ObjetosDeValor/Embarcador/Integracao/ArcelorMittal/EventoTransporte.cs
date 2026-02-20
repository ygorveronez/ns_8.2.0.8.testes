using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal
{
    public class EventoTransporte
    {
        [JsonProperty(PropertyName = "TipoTransporte", Required = Required.Default)]
        public string TipoTransporte { get; set; }

        [JsonProperty(PropertyName = "Transporte", Required = Required.Default)]
        public string Transporte { get; set; }

        [JsonProperty(PropertyName = "Evento", Required = Required.Default)]
        public string Evento { get; set; }

        [JsonProperty(PropertyName = "TrackingTransporte", Required = Required.Default)]
        public string TrackingTransporte { get; set; }

        [JsonProperty(PropertyName = "TipoTracking", Required = Required.Default)]
        public string TipoTracking { get; set; }

        [JsonProperty(PropertyName = "Etapa", Required = Required.Default)]
        public string Etapa { get; set; }

        [JsonProperty(PropertyName = "Data", Required = Required.Default)]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "Hora", Required = Required.Default)]
        public string Hora { get; set; }

        [JsonProperty(PropertyName = "Fornecimentos", Required = Required.Default)]
        public List<Fornecimento> Fornecimentos { get; set; }
    }
}
