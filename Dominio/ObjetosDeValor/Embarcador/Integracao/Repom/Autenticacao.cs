using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class Autenticacao
    {
        [JsonProperty(PropertyName = "usuario", Required = Required.Always)]
        public string Usuario { get; set; }

        [JsonProperty(PropertyName = "senha", Required = Required.Always)]
        public string Senha { get; set; }
    }
}
