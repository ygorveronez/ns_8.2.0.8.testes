using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz
{
    public sealed class RequisicaoIntegracao
    {
        [JsonProperty(PropertyName = "OriginBranch", Required = Required.Default)]
        public string FilialOrigemDescricao { get; set; }

        [JsonProperty(PropertyName = "DocumentOriginBranch", Required = Required.Default, Order = 2)]
        public string FilialOrigemCNPJ { get; set; }

        [JsonProperty(PropertyName = "CityOfOriginBranch", Required = Required.Default, Order = 4)]
        public string FilialOrigemCidade { get; set; }

        [JsonProperty(PropertyName = "FederativeUnitOrigenBranch", Required = Required.Default, Order = 1)]
        public string FilialOrigemUF { get; set; }

        [JsonProperty(PropertyName = "OriginIbgeCodeBranch", Required = Required.Default, Order = 3)]
        public string FilialOrigemIBGE { get; set; }

        [JsonProperty(PropertyName = "DestinationBranch", Required = Required.Default)]
        public string FilialDestinoDescricao { get; set; }

        [JsonProperty(PropertyName = "DocumentDestinationBranch", Required = Required.Default, Order = 7)]
        public string FilialDestinoCNPJ { get; set; }

        [JsonProperty(PropertyName = "CityOfDestinationBranch", Required = Required.Default, Order = 5)]
        public string FilialDestinoCidade { get; set; }

        [JsonProperty(PropertyName = "FederativeUnitDestinationBranch", Required = Required.Default, Order = 6)]
        public string FilialDestinoUF { get; set; }

        [JsonProperty(PropertyName = "DestinationIbgeCodeBranch", Required = Required.Default, Order = 8)]
        public string FilialDestinoIBGE { get; set; }

        [JsonProperty(PropertyName = "Plate", Required = Required.Default, Order = 9)]
        public string PlacaVeiculo { get; set; }

        [JsonProperty(PropertyName = "DocumentShippingCompany", Required = Required.Default, Order = 10)]
        public string CNPJTransportador { get; set; }
    }
}
