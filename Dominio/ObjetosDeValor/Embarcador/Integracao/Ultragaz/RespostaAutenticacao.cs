using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz
{
    public sealed class RespostaAutenticacao
    {
        [JsonProperty(PropertyName = "access_token", Order = 1, Required = Required.Default)]
        public string TokenAcesso { get; set; }

        [JsonProperty(PropertyName = "token_type", Order = 2, Required = Required.Default)]
        public string TipoToken { get; set; }

        [JsonProperty(PropertyName = "expires_in", Order = 3, Required = Required.Default)]
        public int TempoExpiracao { get; set; }
    }
}
