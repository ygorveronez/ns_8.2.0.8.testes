using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unitop
{
    public class LocalizacoesResponse
    {
        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Data")]
        public string Data { get; set; }
    }
}
