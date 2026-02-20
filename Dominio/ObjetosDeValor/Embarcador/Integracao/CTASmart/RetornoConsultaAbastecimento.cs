using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart
{
    public class RetornoConsultaAbastecimento
    {
        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public int Codigo { get; set; }

        [JsonProperty(PropertyName = "data_inicio", Required = Required.Default)]
        public string DataInicio { get; set; }

        [JsonProperty(PropertyName = "hora_inicio", Required = Required.Default)]
        public string HoraInicio { get; set; }

        [JsonProperty(PropertyName = "volume", Required = Required.Default)]
        public string Volume { get; set; }

        [JsonProperty(PropertyName = "odometro", Required = Required.Default)]
        public string Odometro { get; set; }

        [JsonProperty(PropertyName = "horimetro", Required = Required.Default)]
        public string Horimetro { get; set; }

        [JsonProperty(PropertyName = "custo", Required = Required.Default)]
        public string Custo { get; set; }

        [JsonProperty(PropertyName = "custo_unitario", Required = Required.Default)]
        public string CustoUnitario { get; set; }

        [JsonProperty(PropertyName = "combustivel", Required = Required.Default)]
        public RetornoConsultaCombustivel Combustivel { get; set; }

        [JsonProperty(PropertyName = "veiculo", Required = Required.Default)]
        public RetornoConsultaVeiculo Veiculo { get; set; }

        [JsonProperty(PropertyName = "posto", Required = Required.Default)]
        public RetornoConsultaPosto Posto { get; set; }

        [JsonProperty(PropertyName = "motorista", Required = Required.Default)]
        public RetornoConsultaMotorista Motorista { get; set; }
    }
}
