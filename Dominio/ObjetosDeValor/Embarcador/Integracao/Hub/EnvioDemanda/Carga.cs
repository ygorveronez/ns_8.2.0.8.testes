using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public class Carga
    {
        [JsonProperty("number")]
        public string Numero { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
