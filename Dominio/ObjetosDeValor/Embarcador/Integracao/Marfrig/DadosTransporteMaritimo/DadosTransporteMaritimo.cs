using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo
{
    public sealed class DadosTransporteMaritimoRecebimento
    {
        [JsonProperty(PropertyName = "cabecalho", Order = 1, Required = Required.Always)]
        public DadosTransporteMaritimoCabecalho Cabecalho { get; set; }

        [JsonProperty(PropertyName = "container", Order = 2, Required = Required.Always)]
        public DadosTransporteMaritimoContainer Container { get; set; }

        [JsonProperty(PropertyName = "reserva", Order = 3, Required = Required.Always)]
        public DadosTransporteMaritimoReserva Reserva { get; set; }

        [JsonProperty(PropertyName = "rota", Order = 4, Required = Required.Always)]
        public DadosTransporteMaritimoRota Rota { get; set; }

        [JsonProperty(PropertyName = "transporte", Order = 5, Required = Required.Always)]
        public DadosTransporteMaritimoTransporte Transporte { get; set; }

        [JsonProperty(PropertyName = "pcp", Order = 6, Required = Required.Always)]
        public DadosTransporteMaritimoPCP PCP { get; set; }
    }
}
