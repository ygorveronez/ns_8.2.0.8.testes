using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class StatusVeiculo
    {
        [JsonProperty("currentStatus")]
        public Enumeradores.StatusVeiculo? StatusAtual { get; set; }

        [JsonProperty("statusObservation")]
        public string ObservacaoStatus { get; set; }
    }

}
