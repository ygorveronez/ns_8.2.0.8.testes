using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol
{
    public class ResponseErroEventoEntrega
    {

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("error")]
        public Error Error { get; set; }
    }

    public class Error
    {
        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }
    }
}
