using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz
{
    public sealed class RequisicaoVeiculo
    {
        [JsonProperty(PropertyName = "DocumentNumber", Required = Required.Default)]
        public string CnpjTransportador { get; set; }

        [JsonProperty(PropertyName = "ShipCompany", Required = Required.Default)]
        public string RazaoSocialTransportador { get; set; }

        [JsonProperty(PropertyName = "LicencePlate", Required = Required.Default)]
        public string Placa { get; set; }

        [JsonProperty(PropertyName = "LicencePlateUf", Required = Required.Default)]
        public string UF { get; set; }

        [JsonProperty(PropertyName = "VehicleType", Required = Required.Default)]
        public string CodigoIntegracaoModeloVeicular { get; set; }

        [JsonProperty(PropertyName = "Active", Required = Required.Default)]
        public string Ativo { get; set; }

        [JsonProperty(PropertyName = "SourceSystem", Required = Required.Default)]
        public string SistemaOrigem { get; set; }
    }
}
