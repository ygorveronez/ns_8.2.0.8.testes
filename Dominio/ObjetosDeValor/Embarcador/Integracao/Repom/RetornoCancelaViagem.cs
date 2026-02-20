using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class RetornoCancelaViagem
    {
        [JsonProperty(PropertyName = "result", Required = Required.AllowNull)]
        public RetornoIntegracaoResult Result { get; set; }
    }
}
