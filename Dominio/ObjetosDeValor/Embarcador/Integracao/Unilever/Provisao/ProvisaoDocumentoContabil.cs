using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao
{
    public sealed class ProvisaoDocumentoContabil
    {
        [JsonProperty(PropertyName = "item", Order = 1, Required = Required.Always)]
        public ProvisaoDocumentoContabilItem Item { get; set; }
    }
}
