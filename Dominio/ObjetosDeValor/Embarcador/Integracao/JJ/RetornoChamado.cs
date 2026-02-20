using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.JJ
{
    public class RetornoChamado
    {
        [JsonProperty(PropertyName = "idDev", Required = Required.Default)]
        public string Protocolo { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.Default)]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "msgError", Required = Required.Default)]
        public string MensagemErro { get; set; }
    }
}
