using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FS
{
    public class Metadata
    {
        [JsonProperty(PropertyName = "id")]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Tipo { get; set; }
    }

    public class Response<T>
    {
        [JsonProperty(PropertyName = "d")]
        public T Dados { get; set; }
    }
}
