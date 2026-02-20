using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete
{
    public class Token
    {
        [JsonProperty("accessToken")]
        public string AcessToken { get; set; }
    }
}
