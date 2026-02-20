using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class Response
    {
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("message")]
        public string Mensagem { get; set; }
    }
}
