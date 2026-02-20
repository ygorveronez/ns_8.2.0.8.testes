using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Recursos
    {
        [JsonProperty(PropertyName = "peso", Required = Required.Default)]
        public string Peso { get; set; }

        [JsonProperty(PropertyName = "veiculos", Required = Required.Default)]
        public string Veiculos { get; set; }

        [JsonProperty(PropertyName = "unidade_negociacao", Required = Required.Default)]
        public string UnidadeNegociacao { get; set; }
    }
}
