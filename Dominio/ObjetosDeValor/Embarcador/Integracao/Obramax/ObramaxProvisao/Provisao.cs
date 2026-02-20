using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class Provisao
    {
        [JsonProperty("remessa")]
        public string Remessa { get; set; }

        [JsonProperty("km_calculado")]
        public decimal KMCalculado { get; set; }

        [JsonProperty("valor_calculado")]
        public decimal ValorCalculado { get; set; }
    }
}
