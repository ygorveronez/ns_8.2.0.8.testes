using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Simonetti
{
    public class Payload
    {
        [JsonProperty("protocolo_pedido")]
        public string ProtocoloPedido;

        [JsonProperty("itens")]
        public List<Item> Itens;
    }
}
