using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162
{
    public class RetornoVeiculo
    {
        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "error", Required = Required.Default)]
        public bool Erro { get; set; }

        [JsonProperty(PropertyName = "code", Required = Required.Default)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "exist", Required = Required.Default)]
        public bool JaExiste { get; set; }

        [JsonProperty(PropertyName = "car", Required = Required.Default)]
        public Veiculo Veiculo { get; set; }

        [JsonProperty(PropertyName = "data", Required = Required.Default)]
        public Veiculo Data { get; set; }
    }
}
