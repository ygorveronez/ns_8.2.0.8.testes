using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile
{
    public class DadosRetornoWeberChile
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("codigo")]
        public int Codigo { get; set; }

        [JsonProperty("descripcion")]
        public string Descricao { get; set; }
    }
}
