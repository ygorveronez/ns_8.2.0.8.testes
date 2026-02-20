using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Krona
{
    public sealed class KronaServiceCancelamento
    {
        [JsonProperty(PropertyName = "usuario_login", Order = 1, Required = Required.Always)]
        public Autenticacao Autenticacao { get; set; }

        [JsonProperty(PropertyName = "viagem", Order = 9, Required = Required.Always)]
        public ViagemCancelamento Viagem { get; set; }
    }
}