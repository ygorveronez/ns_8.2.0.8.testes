using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Brado
{
    public class TokenResponseLogin
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("ambiente")]
        public string Ambiente { get; set; }
    }
}
