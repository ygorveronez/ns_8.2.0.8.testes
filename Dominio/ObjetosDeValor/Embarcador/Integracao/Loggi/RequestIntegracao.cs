using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi
{
    public class RequestIntegracao
    {
        [JsonProperty("loggi_key")]
        public string LogKeyPacote { get; set; }

        [JsonProperty("protocolo_carga")]
        public int ProtocoloCarga { get; set; }

        [JsonProperty("protocolo_pedido")]
        public string ProtocoloPedido { get; set; }

        [JsonProperty("codigo_operacao")]
        public string CodigoIntegracaoTipoOperacao { get; set; }
    }
}
