using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class RetornoAvancoEmissaoReturn
    {
        [JsonProperty(PropertyName = "errorMessage", Required = Required.AllowNull)]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "errorCode", Required = Required.AllowNull)]
        public int ErrorCode { get; set; }

    }
}
