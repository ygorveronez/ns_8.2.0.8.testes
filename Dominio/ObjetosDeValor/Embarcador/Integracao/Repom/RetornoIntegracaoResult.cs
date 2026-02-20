using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class RetornoIntegracaoResult
    {
        [JsonProperty(PropertyName = "status", Required = Required.AllowNull)]
        public bool Status { get; set; }

        [JsonProperty(PropertyName = "descricao", Required = Required.Default)]
        public string Descricao { get; set; }

        [JsonProperty(PropertyName = "numeroErro", Required = Required.Default)]
        public string NumeroErro { get; set; }
    }
}
