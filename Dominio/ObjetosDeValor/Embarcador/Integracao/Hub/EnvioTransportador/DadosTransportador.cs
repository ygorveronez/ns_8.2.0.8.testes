using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioTransportador
{
    public class DadosTransportador
    {
        [JsonProperty("carrier")]
        public Transportador Transportador { get; set; }

        [JsonProperty("headquarters")]
        public Matriz Matriz { get; set; }

        [JsonProperty("active")]
        public bool Ativo { get; set; }
    }
}
