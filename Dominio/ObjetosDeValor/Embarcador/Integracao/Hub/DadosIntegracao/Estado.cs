using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub
{
    public class Estado
    {
        [JsonProperty("name")]
        public string NomeEstado { get; set; }

        [JsonProperty("code")]
        public string CodigoEstado { get; set; }

        [JsonProperty("abbreviation")]
        public string Abreviacao { get; set; }

        [JsonProperty("country")]
        public Pais Pais { get; set; }
    }
}
