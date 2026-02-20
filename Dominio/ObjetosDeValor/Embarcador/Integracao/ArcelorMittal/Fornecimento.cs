using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal
{
    public class Fornecimento
    {
        [JsonProperty(PropertyName = "NrFornecimento", Required = Required.Default)]
        public string NrFornecimento { get; set; }

        [JsonProperty(PropertyName = "NF", Required = Required.Default)]
        public string NumeroNotaFiscal { get; set; }

        [JsonProperty(PropertyName = "Serie", Required = Required.Default)]
        public string Serie { get; set; }

        [JsonProperty(PropertyName = "TransporteSAP", Required = Required.Default)]
        public string TransporteSAP { get; set; }

        [JsonProperty(PropertyName = "Latitude", Required = Required.Default)]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "Longitude", Required = Required.Default)]
        public string Longitude { get; set; }

        [JsonProperty(PropertyName = "Ocorrencia", Required = Required.Default)]
        public string Ocorrencia { get; set; }

        [JsonProperty(PropertyName = "TipoPrevisao", Required = Required.Default)]
        public string TipoPrevisao { get; set; }

        [JsonProperty(PropertyName = "DataPrevisaoEntregaInicio", Required = Required.Default)]
        public string DataPrevisaoEntregaInicio { get; set; }

        [JsonProperty(PropertyName = "HoraPrevisaoEntregaInicio", Required = Required.Default)]
        public string HoraPrevisaoEntregaInicio { get; set; }

        [JsonProperty(PropertyName = "DataPrevisaoEntregaFim", Required = Required.Default)]
        public string DataPrevisaoEntregaFim { get; set; }

        [JsonProperty(PropertyName = "HoraPrevisaoEntregaFim", Required = Required.Default)]
        public string HoraPrevisaoEntregaFim { get; set; }

        [JsonProperty(PropertyName = "TrackingEntrega", Required = Required.Default)]
        public string TrackingEntrega { get; set; }

        [JsonProperty(PropertyName = "TipoTrackingEntrega", Required = Required.Default)]
        public string TipoTrackingEntrega { get; set; }

        [JsonProperty(PropertyName = "DataAgendEntrega", Required = Required.Default)]
        public string DataAgendaEntrega { get; set; }

        [JsonProperty(PropertyName = "HoraAgendEntrega", Required = Required.Default)]
        public string HoraAgendaEntrega { get; set; }

        [JsonProperty(PropertyName = "TipoOcorrencia", Required = Required.Default)]
        public string TipoOcorrencia { get; set; }

        [JsonProperty(PropertyName = "ImpediuEntrega", Required = Required.Default)]
        public string ImpediuEntrega { get; set; }
    }
}
