using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unitop
{
    public class ErrorResponse
    {
        [JsonProperty("Error")]
        public string Erro { get; set; }

        [JsonProperty("Detalhes")]
        public string Detalhes { get; set; }
    }
}
