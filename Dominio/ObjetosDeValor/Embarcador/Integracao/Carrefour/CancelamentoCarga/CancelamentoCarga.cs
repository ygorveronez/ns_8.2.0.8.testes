using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.CancelamentoCarga
{
    public sealed class CancelamentoCarga
    {
        [JsonProperty(PropertyName = "numeroRomaneio", Required = Required.Always)]
        public string CodigoCargaEmbarcador { get; set; }

        [JsonProperty(PropertyName = "motivoCancelamento", Required = Required.Always)]
        public string MotivoCancelamento { get; set; }

        [JsonProperty(PropertyName = "protocoloCarga", Required = Required.Always)]
        public string ProtocoloCarga { get; set; }
    }
}
