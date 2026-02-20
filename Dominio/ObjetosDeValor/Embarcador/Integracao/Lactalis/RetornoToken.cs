using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Lactalis
{
    public class RetornoToken
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }
    }
}
