using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GSW
{
    public class Autenticacao
    {
        [JsonProperty(PropertyName = "username", Required = Required.Always)]
        public string Usuario { get; set; }

        [JsonProperty(PropertyName = "password", Required = Required.Always)]
        public string Senha { get; set; }
    }
}
