using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class RetornoAvancoEmissao
    {
        [JsonProperty(PropertyName = "return", Required = Required.AllowNull)]
        public RetornoAvancoEmissaoReturn Return { get; set; }
    }
}
