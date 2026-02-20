using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao
{
    public sealed class Provisao
    {
        [JsonProperty(PropertyName = "TKNUM", Order = 1, Required = Required.Always)]
        public string NumeroCarga { get; set; }

        [JsonProperty(PropertyName = "Stages", Order = 2)]
        public ProvisaoStage[] Stages { get; set; }
    }
}
