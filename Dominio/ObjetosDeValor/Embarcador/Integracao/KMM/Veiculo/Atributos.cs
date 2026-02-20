using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class Atributos
    {
        [JsonProperty(PropertyName = "cod_atributo", Required = Required.Default)]
        public string cod_atributo { get; set; }

        [JsonProperty(PropertyName = "valor", Required = Required.Default)]
        public string valor { get; set; }
    }
}