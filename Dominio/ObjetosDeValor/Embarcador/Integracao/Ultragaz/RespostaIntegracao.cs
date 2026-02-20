using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz
{
    public sealed class RespostaIntegracao
    {
        [JsonProperty(PropertyName = "carrier-status", Required = Required.Default)]
        public string RawSituacaoTransportador { get; set; }

        [JsonProperty(PropertyName = "truck-status", Required = Required.Default)]
        public string RawSituacaoVeiculo { get; set; }

        [JsonProperty(PropertyName = "error-message", Required = Required.Default)]
        public string MensagemErro { get; set; }

        [JsonIgnore]
        public bool SituacaoTransportador => RawSituacaoTransportador == "true";

        [JsonIgnore]
        public bool SituacaoVeiculo => RawSituacaoVeiculo == "true";
    }
}
