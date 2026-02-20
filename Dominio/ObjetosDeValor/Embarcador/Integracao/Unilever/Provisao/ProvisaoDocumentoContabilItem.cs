using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao
{
    public sealed class ProvisaoDocumentoContabilItem
    {
        [JsonProperty(PropertyName = "KSCHL", Order = 1, Required = Required.Always)]
        public string Tipo { get; set; }

        [JsonProperty(PropertyName = "KWERT", Order = 2, Required = Required.Always)]
        public string Valor { get; set; }
    }
}
