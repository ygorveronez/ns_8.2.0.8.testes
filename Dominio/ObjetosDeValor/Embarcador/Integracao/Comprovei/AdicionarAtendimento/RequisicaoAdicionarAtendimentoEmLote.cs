using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento
{
    public class RequisicaoAdicionarAtendimentoEmLote
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("protocol")]
        public string Protocol { get; set; }

        [JsonProperty("results")]
        public List<ConfiguracaoAtendimento> Results { get; set; }
    }
}
