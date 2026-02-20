using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub
{
    public class Endereco
    {
        [JsonProperty("type")]
        public int Tipo { get; set; }

        [JsonProperty("street")]
        public string Rua { get; set; }

        [JsonProperty("number")]
        public string Numero { get; set; }

        [JsonProperty("complement")]
        public string Complemento { get; set; }

        [JsonProperty("postalCode")]
        public string CEP { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("polygon")]
        public string Poligono { get; set; }

        [JsonProperty("city")]
        public Cidade Cidade { get; set; }
    }
}
