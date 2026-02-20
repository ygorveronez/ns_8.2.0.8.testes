using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub
{
    public class Tipo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
