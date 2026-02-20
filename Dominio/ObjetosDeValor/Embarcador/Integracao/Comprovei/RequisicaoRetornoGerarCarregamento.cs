using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei
{
    public class RequisicaoRetornoGerarCarregamento
    {
        [JsonProperty("numeroCarregamento")]
        public string NumeroCarregamento { get; set; }

        [JsonProperty("protocoloIntegracao")]
        public string Protocolo { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }

        [JsonProperty("camposPersonalizados")]
        public string CamposPersonalizados { get; set; }
    }
}
