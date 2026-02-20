using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SAP
{
    public class RetornoIntegracaoDadosCarga
    {
        [JsonProperty("SAPDocNum")]
        public string NumeroDocumentoSAP { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Text")]
        public string Texto { get; set; }
    }
}
