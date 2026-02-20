using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCancelamentoCarga.Retorno
{
    public class DadosRetorno
    {
        [JsonProperty(PropertyName = "__metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty(PropertyName = "Input")]
        public InputRetorno Input { get; set; }

        [JsonProperty(PropertyName = "Message")]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "Error")]
        public string Erro { get; set; }
    }
}
