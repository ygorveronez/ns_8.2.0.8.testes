using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class MSPortadora
    {
        [JsonProperty("detail")]
        public Detalhes Detalhes { get; set; }
    }
}
