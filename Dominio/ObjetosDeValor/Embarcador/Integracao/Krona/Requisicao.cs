using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Krona
{
    public sealed class Requisicao
    {
        [JsonProperty(PropertyName = "kronaService", Order = 1, Required = Required.Always)]
        public KronaService KronaService { get; set; }
    }
}
