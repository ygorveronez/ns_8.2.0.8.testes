using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class IntegracaoPedido
    {
        [JsonProperty(PropertyName = "orders")]
        public List<Pedido> Pedidos { get; set; }
    }
}
