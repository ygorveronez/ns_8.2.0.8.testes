using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170
{
    public class envLogin
    {
        [JsonProperty(PropertyName = "username", Required = Required.Default)]
        public string username { get; set; }

        [JsonProperty(PropertyName = "password", Required = Required.Default)]
        public string password { get; set; }
    }
}