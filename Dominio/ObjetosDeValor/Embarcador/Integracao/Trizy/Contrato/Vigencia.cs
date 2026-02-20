using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Vigencia
    {
        [JsonProperty(PropertyName = "inicio", Required = Required.Default)]
        public string Inicio { get; set; }

        [JsonProperty(PropertyName = "fim", Required = Required.Default)]
        public string Fim { get; set; }
    }
}
