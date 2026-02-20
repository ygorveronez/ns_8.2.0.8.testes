using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCancelamentoCarga.Requisicao
{
    public class CancelamentoCarga
    {
        [JsonProperty(PropertyName = "PROTOCOLO_CARGA")]
        public string Protocolo { get; set; }

        [JsonProperty(PropertyName = "MOTIVO_CANC")]
        public string Motivo { get; set; }
    }
}
