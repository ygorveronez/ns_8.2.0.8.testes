using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido
{
    public class ConfiguracaoPedido
    {
        [JsonProperty("numeroPedidoEmbarcador")]
        public string NumeroPedido { get; set; }

        [JsonProperty("protocoloIntegracaoPedido")]
        public int Protocolo { get; set; }

        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }

        [JsonProperty("statusEvento")]
        public string StatusEvento { get; set; }
    }
}
