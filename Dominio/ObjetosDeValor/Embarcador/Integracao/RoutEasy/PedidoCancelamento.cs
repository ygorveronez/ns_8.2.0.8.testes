using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class PedidoCancelamento
    {
        [JsonProperty(PropertyName = "site")]
        public string CodigoIntegracaoFilial { get; set; }

        [JsonProperty(PropertyName = "order_number")]
        public string NumeroPedido { get; set; }
    }
}
