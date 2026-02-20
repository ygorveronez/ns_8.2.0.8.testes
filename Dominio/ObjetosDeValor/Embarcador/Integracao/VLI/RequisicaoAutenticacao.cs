using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VLI
{
    public sealed class RequisicaoAutenticacao
    {
        [JsonProperty(PropertyName = "grant_type", Order = 1, Required = Required.Default)]
        public string TipoAutenticacao { get; set; }
    }
}
