using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm
{
    public class RequisicaoLogin
    {
        [JsonPropertyName("login")]
        public string Usuario { get; set; }

        [JsonPropertyName("password")]
        public string Senha { get; set; }
    }
}
