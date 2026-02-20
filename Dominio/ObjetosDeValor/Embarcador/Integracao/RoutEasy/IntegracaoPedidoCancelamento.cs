using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class IntegracaoPedidoCancelamento
    {
        [JsonProperty(PropertyName = "orders")]
        public List<PedidoCancelamento> Pedidos { get; set; }
    }
}
