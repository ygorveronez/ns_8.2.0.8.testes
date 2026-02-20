using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class VeiculoOferta
    {
        [JsonProperty("sequence")]
        public int Sequencia { get; set; }

        [JsonProperty("identity")]
        public string Identidade { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("nextAvailabilityDate")]
        public DateTime? ProximaDataDisponibilidade { get; set; }

        [JsonProperty("nextAvailabilityCityId")]
        public string IdProximaCidadeDisponibilidade { get; set; }

        [JsonProperty("reliabilityPercentage")]
        public decimal? PercentualConfiabilidade { get; set; }

        [JsonProperty("vehicles")]
        public Veiculo Veiculo { get; set; }


    }
}
