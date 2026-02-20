using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk
{
    public class RequestCancelamento
    {
        [JsonProperty("monitoramento_id")]
        public string ProtocoloCancelamento { get; set; }

        [JsonProperty("cancelamento_motivo")]
        public string MotivoCancelamento { get; set; }
    }
}
