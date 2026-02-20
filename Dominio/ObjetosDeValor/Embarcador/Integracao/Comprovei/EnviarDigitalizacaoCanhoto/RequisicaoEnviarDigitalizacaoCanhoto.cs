using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.EnviarDigitalizacaoCanhoto
{
    public class RequisicaoEnviarDigitalizacaoCanhoto
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
        public List<ConfiguracaoCanhoto> Results { get; set; }
    }

}
