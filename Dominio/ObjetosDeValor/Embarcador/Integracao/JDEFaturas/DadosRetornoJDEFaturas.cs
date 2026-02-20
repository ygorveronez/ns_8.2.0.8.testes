using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas
{
    public class DadosRetornoJDEFaturas
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("fase")]
        public string Fase { get; set; }

        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }
    }
}
