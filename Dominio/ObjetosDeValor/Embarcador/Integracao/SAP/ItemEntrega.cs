using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SAP
{
    public class ItemEntrega
    {
        [JsonProperty("DeliverySequence")]
        public string SequenciaEntrega { get; set; }

        [JsonProperty("Delivery")]
        public string Entrega { get; set; }

        [JsonProperty("ExpectedDate")]
        public string DataEsperada { get; set; }

        [JsonProperty("EstimatedDate")]
        public string DataEstimada { get; set; }

        [JsonProperty("LoadingDate")]
        public string DataCarregamento { get; set; }

        [JsonProperty("Latitude")]
        public string Latitude { get; set; }

        [JsonProperty("Longitude")]
        public string Longitude { get; set; }
    }
}
