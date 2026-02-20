using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Lactalis
{
    public class Pedido
    {
        [JsonProperty("protocoloIntegracaoPedido")]
        public string ProtocoloIntegracaoPedido { get; set; }

    }
}
