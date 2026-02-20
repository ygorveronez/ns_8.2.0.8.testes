using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin
{
    public class RespostaIntegracao
    {
        [JsonProperty(PropertyName = "status", Required = Required.Default)]
        public string Status;
        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public string Message;
        [JsonProperty(PropertyName = "messageType", Required = Required.Default)]
        public string MessageType;
        [JsonProperty(PropertyName = "data", Required = Required.Default)]
        public RespostaData Dados { get; set; }
        [JsonProperty(PropertyName = "timestamp", Required = Required.Default)]
        public string Timestamp;
    }
}
