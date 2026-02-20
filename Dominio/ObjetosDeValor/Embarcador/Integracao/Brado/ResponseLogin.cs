using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Brado
{
    public class ResponseLogin
    {
        [JsonProperty("success")]
        public bool Sucesso { get; set; }

        [JsonProperty("code")]
        public int Codigo { get; set; }

        [JsonProperty("result")]
        public TokenResponseLogin Token { get; set; }


    }
}
