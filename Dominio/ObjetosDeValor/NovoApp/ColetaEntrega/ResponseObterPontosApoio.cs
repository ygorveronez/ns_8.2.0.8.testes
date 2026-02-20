using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public partial class ResponseObterPontosApoio
    {
        [JsonProperty("CodigoIntegracao")]
        public string CodigoIntegracao { get; set; }

        [JsonProperty("RaioEmMetros")]
        public long RaioEmMetros { get; set; }

        [JsonProperty("RazaoSocial")]
        public string RazaoSocial { get; set; }

        [JsonProperty("Endereco")]
        public Endereco Endereco { get; set; }

        [JsonProperty("NomeFantasia")]
        public string NomeFantasia { get; set; }

        [JsonProperty("Telefone")]
        public Telefone Telefone { get; set; }
    }

    public partial class Telefone
    {
        [JsonProperty("CodigoPais")]
        public string CodigoPais { get; set; }

        [JsonProperty("Numero")]
        public string Numero { get; set; }
    }

    public partial class Endereco
    {
        [JsonProperty("Cidade")]
        public string Cidade { get; set; }

        [JsonProperty("Uf")]
        public string Uf { get; set; }

        [JsonProperty("Latitude")]
        public string Latitude { get; set; }

        [JsonProperty("Longitude")]
        public string Longitude { get; set; }
    }
}
