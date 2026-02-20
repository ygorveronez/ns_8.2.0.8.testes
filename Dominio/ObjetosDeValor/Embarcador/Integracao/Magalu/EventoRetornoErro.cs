using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Magalu
{
    public class EventoRetornoErro
    {
        [JsonProperty(PropertyName = "developerMessage", Required = Required.Default)]
        public string MensagemDesenvolvedor { get; set; }

        [JsonProperty(PropertyName = "userMessage", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "errorCode", Required = Required.Default)]
        public int CodigoErro { get; set; }

        [JsonProperty(PropertyName = "moreInfo", Required = Required.Default)]
        public string MaisInformacao { get; set; }
    }
}
