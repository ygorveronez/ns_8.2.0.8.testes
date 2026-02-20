using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class PedidoItem
    {
        [JsonProperty(PropertyName = "code")]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Descricao { get; set; }

        [JsonProperty(PropertyName = "qty")]
        public decimal Quantidade { get; set; }
    }
}
