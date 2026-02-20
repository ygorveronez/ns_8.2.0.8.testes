using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class Detalhes
    {
        [JsonProperty("CSTATUS")]
        public string CSTATUS { get; set; }

        [JsonProperty("CREASON")]
        public string CREASON { get; set; }

        [JsonProperty("CLCHANGED")]
        public string CLCHANGED { get; set; }

        [JsonProperty("CLCHANGET")]
        public string CLCHANGET { get; set; }
    }
}
