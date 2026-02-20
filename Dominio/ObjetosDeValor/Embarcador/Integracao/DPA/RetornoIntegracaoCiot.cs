using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DPA
{
    public class RetornoIntegracaoCiot
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Mensagem { get; set; }
    }
}
