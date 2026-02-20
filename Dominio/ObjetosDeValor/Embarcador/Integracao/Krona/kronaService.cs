using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Krona
{
    public sealed class KronaService
    {
        [JsonProperty(PropertyName = "usuario_login", Order = 1, Required = Required.Always)]
        public Autenticacao Autenticacao { get; set; }

        [JsonProperty(PropertyName = "destinos", Order = 8, Required = Required.Always)]
        public Destino Destinos { get; set; }

        [JsonProperty(PropertyName = "motorista_1", Order = 3, Required = Required.Always)]
        public Motorista Motorista { get; set; }

        [JsonProperty(PropertyName = "origem", Order = 7, Required = Required.Always)]
        public Entidade Origem { get; set; }

        [JsonProperty(PropertyName = "reboque_1", Order = 5)]
        public Veiculo Reboque { get; set; }

        [JsonProperty(PropertyName = "reboque_2", Order = 6)]
        public Veiculo SegundoReboque { get; set; }

        [JsonProperty(PropertyName = "transportador", Order = 2, Required = Required.Always)]
        public Entidade Transportador { get; set; }

        [JsonProperty(PropertyName = "veiculo", Order = 4, Required = Required.Always)]
        public Veiculo Veiculo { get; set; }

        [JsonProperty(PropertyName = "viagem", Order = 9, Required = Required.Always)]
        public Viagem Viagem { get; set; }
    }
}