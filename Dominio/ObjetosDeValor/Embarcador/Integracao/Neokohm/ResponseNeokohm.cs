using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm
{
    public class ResponseNeokohm
    {

        [JsonProperty("codigo")]
        public int Codigo { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("numcarga")]
        public string NumCarga { get; set; }
    }
}
