using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Krona
{
    public sealed class ViagemCancelamento
    {
        [JsonProperty(PropertyName = "id", Order = 1)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "cancelar", Order = 2)]
        public bool Cancelar { get; set; }

        [JsonProperty(PropertyName = "motivo_cancelamento", Order = 3)]
        public string MotivoCancelamento { get; set; }
    }
}
