using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class FracoesCota
    {
        [JsonProperty(PropertyName = "data", Required = Required.Default)]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "peso", Required = Required.Default)]
        public string Peso { get; set; }

        [JsonProperty(PropertyName = "transportador", Required = Required.Default)]
        public Transportador Transportador { get; set; }
    }
}
