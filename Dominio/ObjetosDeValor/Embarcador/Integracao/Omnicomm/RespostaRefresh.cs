using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm
{
    public class RespostaRefresh : RespostaLogin
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
    }
}
