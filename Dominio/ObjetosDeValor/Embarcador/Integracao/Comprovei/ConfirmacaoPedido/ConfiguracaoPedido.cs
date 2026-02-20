using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.ConfirmacaoPedido
{
    public class ConfiguracaoPedido
    {
        [JsonProperty("numeroPedidoEmbarcador")]
        public string NumeroPedido { get; set; }

        [JsonProperty("protocoloIntegracaoPedido")]
        public int Protocolo { get; set; }

        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }
    }
}
