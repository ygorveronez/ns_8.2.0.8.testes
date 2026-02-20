using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento
{
    public sealed class Recebimento
    {
        [JsonProperty(PropertyName = "cabecalho", Order = 1, Required = Required.Always)]
        public RecebimentoCabecalho Cabecalho { get; set; }

        [JsonProperty(PropertyName = "container", Order = 2, Required = Required.Always)]
        public RecebimentoContainer Container { get; set; }

        [JsonProperty(PropertyName = "reserva", Order = 3, Required = Required.Always)]
        public RecebimentoReserva Reserva { get; set; }

        [JsonProperty(PropertyName = "rota", Order = 4, Required = Required.Always)]
        public RecebimentoRota Rota { get; set; }

        [JsonProperty(PropertyName = "transporte", Order = 5, Required = Required.Always)]
        public RecebimentoTransporte Transporte { get; set; }
    }
}
