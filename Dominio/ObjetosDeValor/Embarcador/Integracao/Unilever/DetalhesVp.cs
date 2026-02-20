using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class DetalhesVp
    {
        [JsonProperty("detail")]
        public  DetalhesVpItem DetalhesVpItem { get; set; }
    }
}
