using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario
{
    public sealed class PEP
    {
        [JsonProperty(PropertyName = "codigo", Order = 1, Required = Required.Always)]
        public string Codigo { get; set; }
    }
}
