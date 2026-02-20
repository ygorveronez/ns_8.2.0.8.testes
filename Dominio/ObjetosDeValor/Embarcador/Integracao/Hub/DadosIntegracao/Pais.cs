using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub
{
    public class Pais
    {
        [JsonProperty("name")]
        public string NomePais { get; set; }

        [JsonProperty("code")]
        public string CodigoPais { get; set; }
    }
}
