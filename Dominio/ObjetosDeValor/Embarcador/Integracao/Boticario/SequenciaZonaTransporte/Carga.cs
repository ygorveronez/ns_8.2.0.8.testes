using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte
{
    public class Carga
    {
        [JsonProperty(PropertyName = "ordem", Order = 1, Required = Required.Always)]
        public string Ordem { get; set; }

        [JsonProperty(PropertyName = "numeroCarga", Order = 2, Required = Required.Always)]
        public string NumeroCarga { get; set; }

        [JsonProperty(PropertyName = "numeroPedido", Order = 3, Required = Required.Always)]
        public string NumeroPedido { get; set; }

        [JsonProperty(PropertyName = "protocoloCarga", Order = 4, Required = Required.Always)]
        public string ProtocoloCarga { get; set; }

        [JsonProperty(PropertyName = "protocoloPedido", Order = 5, Required = Required.Always)]
        public string ProtocoloPedido { get; set; }
    }
}
