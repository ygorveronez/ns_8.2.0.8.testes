using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public partial class Validacao
    {
        [JsonProperty("vehiclesValidated")]
        public bool VeiculosValidados { get; set; }

        [JsonProperty("vehiclesValidatedObservation")]
        public string ObservacaoVeiculosValidados { get; set; }

        [JsonProperty("crewValidated")]
        public bool TripulacaoValidada { get; set; }

        [JsonProperty("crewValidatedObservation")]
        public string ObservacaoTripulacaoValidada { get; set; }
    }
}
