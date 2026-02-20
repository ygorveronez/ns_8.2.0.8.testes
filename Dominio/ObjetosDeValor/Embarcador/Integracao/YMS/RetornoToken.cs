using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.YMS
{
    public class RetornoToken
    {

        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}
