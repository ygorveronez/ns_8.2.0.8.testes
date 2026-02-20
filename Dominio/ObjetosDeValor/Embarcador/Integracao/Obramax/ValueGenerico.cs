using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class ValueGenerico<T>
    {
        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("data")]
        public T Dados { get; set; }
    }
}
