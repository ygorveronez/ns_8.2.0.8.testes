using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162
{
    public class Veiculo
    {
        [JsonProperty(PropertyName = "company_id", Required = Required.Default)]
        public string CompanyID { get; set; }

        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "car_plate", Required = Required.Default)]
        public string Placa { get; set; }

        [JsonProperty(PropertyName = "chassi", Required = Required.Default)]
        public string Chassi { get; set; }

        [JsonProperty(PropertyName = "renavam", Required = Required.Default)]
        public string Renavam { get; set; }

        [JsonProperty(PropertyName = "prefixo_frota", Required = Required.Default)]
        public string PrefixoFrota { get; set; }
    }
}
