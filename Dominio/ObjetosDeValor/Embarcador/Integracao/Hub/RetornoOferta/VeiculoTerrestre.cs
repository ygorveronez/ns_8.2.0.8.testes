using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class VeiculoTerrestre
    {
        [JsonProperty("plate")]
        public string Placa { get; set; }

        [JsonProperty("identificationNumber")]
        public string NumeroIdentificacao { get; set; }

        [JsonProperty("nationalIdentificationNumber")]
        public string NumeroIdentificacaoNacional { get; set; }

        [JsonProperty("emptyWeight")]
        public int? PesoVazio { get; set; }

        [JsonProperty("axleCount")]
        public int? QuantidadeEixos { get; set; }

        [JsonProperty("axleSuspendedCount")]
        public int? QuantidadeEixosSuspensos { get; set; }

        [JsonProperty("city")]
        public Cidade Cidade { get; set; }
    }
}
