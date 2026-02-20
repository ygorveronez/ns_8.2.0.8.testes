using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat
{
    public class RespostaLogin
    {
        [JsonPropertyName("AccessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("ExpiresIn")]
        public long ExpiresIn { get; set; }
    }
}
