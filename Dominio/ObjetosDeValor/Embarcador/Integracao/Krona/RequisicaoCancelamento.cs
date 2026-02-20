using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Krona
{
    public sealed class RequisicaoCancelamento
    {
        [JsonProperty(PropertyName = "kronaService", Order = 1, Required = Required.Always)]
        public KronaServiceCancelamento KronaService { get; set; }
    }
}
