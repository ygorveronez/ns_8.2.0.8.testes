using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Atributos
    {
        [JsonProperty(PropertyName = "atributo", Required = Required.Default)]
        public string Atributo { get; set; }

        [JsonProperty(PropertyName = "valor", Required = Required.Default)]
        public string Valor { get; set; }
    }
}
