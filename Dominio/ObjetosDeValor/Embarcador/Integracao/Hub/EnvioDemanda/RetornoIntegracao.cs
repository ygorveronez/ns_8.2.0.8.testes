using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public class RetornoIntegracao
    {
        [JsonProperty("succeeded")]
        public bool Sucesso { get; set; }

        [JsonProperty("errors")]
        public List<string> Erros { get; set; }

        [JsonProperty("data")]
        public string ProtocoloIntegracao { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMensagem { get; set; }
    }
}
