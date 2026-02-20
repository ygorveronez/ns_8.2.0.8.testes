using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class RetornoContrato
    {
        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public int Codigo { get; set; }

        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public string Mensagem { get; set; }

    }
}
