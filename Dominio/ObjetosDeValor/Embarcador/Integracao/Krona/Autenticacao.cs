using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Krona
{
    public sealed class Autenticacao
    {
        [JsonProperty(PropertyName = "login", Order = 1, Required = Required.Always)]
        public string Login { get; set; }

        [JsonProperty(PropertyName = "senha", Order = 2, Required = Required.Always)]
        public string Senha { get; set; }
    }
}
