using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class Value
    {
        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("data")]
        public DadosIntegracao Dados { get; set; }
    }
}
