using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class ValidacaoRiscoOferta
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("vehicleValidated")]
        public bool VeiculoValidado { get; set; }

        [JsonProperty("vehicleValidatedMesage")]
        public string MensagemVeiculoValidado { get; set; }

        [JsonProperty("crewValidated")]
        public bool EquipeValidada { get; set; }

        [JsonProperty("crewValidatedMesage")]
        public string MensagemEquipeValidada { get; set; }
    }
}
