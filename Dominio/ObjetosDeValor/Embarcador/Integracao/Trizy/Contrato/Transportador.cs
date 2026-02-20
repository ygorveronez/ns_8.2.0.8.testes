using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Transportador
    {
        [JsonProperty(PropertyName = "cnpj", Required = Required.Default)]
        public string CNPJ { get; set; }

        [JsonProperty(PropertyName = "nome", Required = Required.Default)]
        public string Nome { get; set; }

        [JsonProperty(PropertyName = "nome_fantasia", Required = Required.Default)]
        public string NomeFantasia { get; set; }

        [JsonProperty(PropertyName = "cep", Required = Required.Default)]
        public string CEP { get; set; }
    }
}
