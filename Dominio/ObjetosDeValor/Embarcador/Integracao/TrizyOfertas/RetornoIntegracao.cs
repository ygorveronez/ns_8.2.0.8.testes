using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas
{
    public class RetornoIntegracao
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("freight")]
        public FreightResult Result { get; set; }
    }

    public class FreightResult
    {
        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }
    }
}