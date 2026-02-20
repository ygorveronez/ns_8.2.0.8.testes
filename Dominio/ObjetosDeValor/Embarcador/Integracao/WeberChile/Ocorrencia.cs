using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile
{
    public class Ocorrencia
    {
        [JsonProperty("descripcion")]
        public string Descricao { get; set; }

        [JsonProperty("valor")]
        public decimal Valor { get; set; }
    }
}
