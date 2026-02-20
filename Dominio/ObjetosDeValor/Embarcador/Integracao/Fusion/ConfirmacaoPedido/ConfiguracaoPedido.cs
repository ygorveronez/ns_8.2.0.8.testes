using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoPedido
{
    public class ConfiguracaoPedido
    {
        [JsonProperty("filialIntegracao")]
        public string Filial { get; set; }
        
        [JsonProperty("numeroPedidoEmbarcador")]
        public string NumeroPedido { get; set; }

        [JsonProperty("protocoloIntegracaoPedido")]
        public int Protocolo { get; set; }

        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }
    }
}
