using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Protheus
{
    public class Login
    {
        [JsonProperty(PropertyName = "username", Required = Required.Default)]
        public string Usuario { get; set; }

        [JsonProperty(PropertyName = "password", Required = Required.Default)]
        public string Senha { get; set; }
    }
}
