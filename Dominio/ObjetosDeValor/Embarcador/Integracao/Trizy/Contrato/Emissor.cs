using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Emissor
    {
        [JsonProperty(PropertyName = "cidade", Required = Required.Default)]
        public string Cidade { get; set; }

        [JsonProperty(PropertyName = "cnpj", Required = Required.Default)]
        public string CNPJ { get; set; }

        [JsonProperty(PropertyName = "logradouro", Required = Required.Default)]
        public string Logradouro { get; set; }

        [JsonProperty(PropertyName = "nome", Required = Required.Default)]
        public string Nome { get; set; }

        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "uf", Required = Required.Default)]
        public string UF { get; set; }
    }
}
