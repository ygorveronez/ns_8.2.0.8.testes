using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class PessoaEstrangeira
    {
        [JsonProperty(PropertyName = "nome", Required = Required.Default)]
        public string nome { get; set; }

        [JsonProperty(PropertyName = "nome_fantasia", Required = Required.Default)]
        public string nome_fantasia { get; set; }

        [JsonProperty(PropertyName = "tipo_documento", Required = Required.Default)]
        public string tipo_documento { get; set; }

        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string numero { get; set; }
    }
}