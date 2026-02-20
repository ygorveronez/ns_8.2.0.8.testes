using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senior
{
    public class Retorno
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiraEm { get; set; }

        [JsonProperty("message")]
        public string Mensagem { get; set; }

        [JsonProperty("error")]
        public bool PossuiErro { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }
}
