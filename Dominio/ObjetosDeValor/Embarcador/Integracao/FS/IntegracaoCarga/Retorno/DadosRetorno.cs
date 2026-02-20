using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCarga.Retorno
{
    public class DadosRetorno
    {
        [JsonProperty(PropertyName = "__metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty(PropertyName = "Input")]
        public InputRetorno Input { get; set; }

        [JsonProperty(PropertyName = "Agendamento")]
        public string Agendamento { get; set; }

        [JsonProperty(PropertyName = "Erro")]
        public string Erro { get; set; }
    }
}
