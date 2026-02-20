using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa
{
    public class RequestMarisaLocalidadeCliente
    {
        [JsonProperty("city")]
        public string Cidade;

        [JsonProperty("quarter")]
        public string Bairro;

        [JsonProperty("additional")]
        public string Adicionais;

        [JsonProperty("reference")]
        public string Referencia;

        [JsonProperty("description")]
        public string Descricao;

        [JsonProperty("number")]
        public string Numero;

        [JsonProperty("longitude")]
        public string Longitude;

        [JsonProperty("address")]
        public string Endereco;

        [JsonProperty("latitude")]
        public string Latitude;

        [JsonProperty("state_code")]
        public string CodigoEstado;

        [JsonProperty("zip_code")]
        public string CodigoPostal;
    }
}