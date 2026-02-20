using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioTransportador
{
    public class RetornoIntegracao
    {
        [JsonProperty("succeeded")]
        public bool Sucesso { get; set; }

        [JsonProperty("errors")]
        public List<object> Erros { get; set; }

        [JsonProperty("data")]
        public object Dados { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMensagem { get; set; }
    }
}
