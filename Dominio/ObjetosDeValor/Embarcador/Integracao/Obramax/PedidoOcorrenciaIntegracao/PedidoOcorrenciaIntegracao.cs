using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class PedidoOcorrenciaIntegracao
    {
        [JsonProperty("key")]
        public Key Chave { get; set; }

        [JsonProperty("value")]
        public ValoresPedido ValoresIntegracao { get; set; }
    }
}
