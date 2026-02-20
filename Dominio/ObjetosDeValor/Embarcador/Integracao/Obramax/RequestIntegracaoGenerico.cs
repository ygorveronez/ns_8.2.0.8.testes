using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class RequestIntegracaoGenerico<T>
    {
        [JsonProperty("key")]
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key Chave { get; set; }

        [JsonProperty("value")]
        public ValueGenerico<T> ValoresIntegracao { get; set; }
    }
}
