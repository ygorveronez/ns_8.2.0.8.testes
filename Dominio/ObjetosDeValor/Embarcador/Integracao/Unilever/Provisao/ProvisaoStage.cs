using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao
{
    public sealed class ProvisaoStage
    {
        [JsonProperty(PropertyName = "TSNUM", Order = 1, Required = Required.Always)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "MWSKZ", Order = 2)]
        public string ImpostoValorAgregado { get; set; }

        [JsonProperty(PropertyName = "KOMOK", Order = 3)]
        public string KOMOK { get; set; }

        [JsonProperty(PropertyName = "condition", Order = 4)]
        public ProvisaoDocumentoContabil[] DocumentosContabeis { get; set; }
    }
}
