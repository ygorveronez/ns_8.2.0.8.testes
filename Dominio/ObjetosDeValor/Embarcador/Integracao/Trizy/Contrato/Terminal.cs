using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Terminal
    {
        [JsonProperty(PropertyName = "cnpj", Required = Required.Default)]
        public string CNPJ { get; set; }
    }
}
