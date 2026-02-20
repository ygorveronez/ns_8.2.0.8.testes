using Newtonsoft.Json;
using System;
using System.Collections;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class RetornoPadrao
    {
        [JsonProperty(PropertyName = "success", Required = Required.Default)]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "code", Required = Required.Default)]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public String Message { get; set; }

        [JsonProperty(PropertyName = "details", Required = Required.Default)]
        public String Details { get; set; }

        [JsonProperty(PropertyName = "result", Required = Required.Default)]
        public Hashtable Result { get; set; }
    }
}
