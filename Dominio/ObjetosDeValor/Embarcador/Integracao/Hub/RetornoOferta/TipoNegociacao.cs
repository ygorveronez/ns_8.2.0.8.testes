using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class Tipo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Nome { get; set; }
    }
}
