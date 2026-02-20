using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario
{
    public sealed class CentroCusto
    {
        [JsonProperty(PropertyName = "codigo", Order = 1, Required = Required.AllowNull)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "numeroContaRazao", Order = 2, Required = Required.AllowNull)]
        public string NumeroContaRazao { get; set; }
    }
}
