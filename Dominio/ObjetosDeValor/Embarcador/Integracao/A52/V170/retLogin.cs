using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170
{
    public class retLogin
    {
        [JsonProperty(PropertyName = "statusCode", Required = Required.Default)]
        public int? statusCode { get; set; }

        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public string message { get; set; }

        [JsonProperty(PropertyName = "access_token", Required = Required.Default)]
        public string access_token { get; set; }
    }
}

