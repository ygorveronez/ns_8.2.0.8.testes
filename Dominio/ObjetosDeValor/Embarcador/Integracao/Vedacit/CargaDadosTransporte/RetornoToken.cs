using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte
{
    public class RetornoToken
    {
        [JsonProperty("access_token")]
        public string TokenAcesso { get; set; }

        [JsonProperty("token_type")]
        public string Tipo { get; set; }

        [JsonProperty("expires_in")]
        public int TempoValido { get; set; }
    }
}
