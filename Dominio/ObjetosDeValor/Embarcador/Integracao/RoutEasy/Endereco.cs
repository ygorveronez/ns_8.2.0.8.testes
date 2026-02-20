using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class Endereco
    {
        [JsonProperty(PropertyName = "route")]
        public string Logradouro { get; set; }

        [JsonProperty(PropertyName = "street_number")]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "neighborhood")]
        public string Bairro { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string Localidade { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string Estado { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Pais { get; set; }

        [JsonProperty(PropertyName = "postal_code")]
        public string Cep { get; set; }

        [JsonProperty(PropertyName = "additional_info")]
        public string InformacoesAdicionais { get; set; }

        [JsonProperty(PropertyName = "geocode")]
        public Geolocalizacao Geolocalizacao { get; set; }
    }

    public class Geolocalizacao
    {
        [JsonProperty(PropertyName = "lat")]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public string Longitude { get; set; }
    }
}
