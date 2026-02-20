using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart
{
    public class RetornoConsultaPosto
    {
        [JsonProperty(PropertyName = "codigo", Required = Required.Default)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "nome", Required = Required.Default)]
        public string Nome { get; set; }

        [JsonProperty(PropertyName = "cnpj", Required = Required.Default)]
        public string Cnpj { get; set; }
    }
}
