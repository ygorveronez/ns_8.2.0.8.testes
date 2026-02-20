using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FS
{
    public class RetornoToken
    {
        [JsonProperty(PropertyName = "access_token")]
        public string Token { get; set; }
    }
}
