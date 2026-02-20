using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class PedidoCancelamento
    {
        [JsonProperty("deliveryNumber")]
        public string NumeroPedido { get; set; }

        [JsonProperty("protocolNumber")]
        public string ProtocoloPedido { get; set; }

        [JsonProperty("eventStatus")]
        public string SituacaoDoPedido { get { return "Cancelled"; } }
    }
}
