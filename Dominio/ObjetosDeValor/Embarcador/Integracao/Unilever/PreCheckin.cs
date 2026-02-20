using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class PreCheckin
    {
        [JsonProperty("detail")]
        public DetalhesPreCheckin Detail { get; set; }

    }
}
