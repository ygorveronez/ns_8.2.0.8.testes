using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Simonetti
{
    public class RootRequest
    {
        [JsonProperty("uuid")]
        public string UUID;

        [JsonProperty("origem_nome")]
        public string OrigemNome;

        [JsonProperty("origem_id")]
        public string OrigemId;

        [JsonProperty("payload")]
        public Payload Payload;
    }
}
