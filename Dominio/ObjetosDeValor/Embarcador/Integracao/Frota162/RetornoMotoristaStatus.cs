using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162
{
    public class RetornoMotoristaStatus
    {
        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "error", Required = Required.Default)]
        public bool Erro { get; set; }

        [JsonProperty(PropertyName = "code", Required = Required.Default)]
        public string Codigo { get; set; }
    }
}
