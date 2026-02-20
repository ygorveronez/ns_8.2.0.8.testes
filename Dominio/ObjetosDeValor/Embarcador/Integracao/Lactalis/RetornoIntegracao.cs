using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Lactalis
{
    public class RetornoIntegracao
    {
        [JsonProperty("status")]
        public string Situacao { get; set; }

        [JsonProperty("message")]
        public string Mensagem { get; set; }
    }
}
