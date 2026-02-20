using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete
{
    public class Login
    {
        [JsonProperty("login")]
        public string Usuario { get; set; }

        [JsonProperty("password")]
        public string Senha { get; set; }

    }
}
