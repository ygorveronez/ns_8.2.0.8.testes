using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class DetalhesFrete
    {
        [JsonProperty("item")]
        public Item Item { get; set; }
    }
}
