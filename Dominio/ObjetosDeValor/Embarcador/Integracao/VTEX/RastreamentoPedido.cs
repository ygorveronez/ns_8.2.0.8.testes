using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX
{
    public class RastreamentoPedido
    {
        [JsonProperty(PropertyName = "isDelivered", Required = Required.Always)]
        public bool EstaEntregue { get; set; }

        [JsonProperty(PropertyName = "events", Required = Required.Always)]
        public List<RastreamentoPedidoEvento> Eventos { get; set; }
    }
}
