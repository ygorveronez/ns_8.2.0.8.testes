

using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class MSFRETE
    {
        [JsonProperty("detail")]
        public DetalhesFrete DetalhesFrete { get; set; }
    }
}
