using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm
{
    public class RespostaLogin
    {
        [JsonPropertyName("jwt")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh")]
        public string RefreshToken { get; set; }
    }
}
