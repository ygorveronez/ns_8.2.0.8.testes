using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class DetalhesPreCheckin
    {

        [JsonProperty("PCSTATUS")]
        public string PCSTATUS { get; set; }

        [JsonProperty("PCDETAILS")]
        public string PCDETAILS { get; set; }

        [JsonProperty("PCLCHANGED")]
        public string PCLCHANGED { get; set; }

        [JsonProperty("PCLCHANGET")]
        public string PCLCHANGET { get; set; }
    }
}
