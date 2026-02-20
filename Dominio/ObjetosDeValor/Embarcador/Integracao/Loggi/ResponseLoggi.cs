using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi
{
    public class ResponseLoggi
    {

        [JsonProperty("codigo")]
        public int Codigo { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

    }
}
