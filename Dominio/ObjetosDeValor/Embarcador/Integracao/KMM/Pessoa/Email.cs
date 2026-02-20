using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class Email
    {
        [JsonProperty(PropertyName = "operation", Required = Required.Default)]
        public string Operation { get; set; }

        [JsonProperty(PropertyName = "id_externo", Required = Required.Default)]
        public string IdExterno { get; set; }

        [JsonProperty(PropertyName = "email_id", Required = Required.Default)]
        public string EmailId { get; set; }

        [JsonProperty(PropertyName = "email_padrao", Required = Required.Default)]
        public bool EmailPadrao { get; set; }

        [JsonProperty(PropertyName = "username", Required = Required.Default)]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "provedor", Required = Required.Default)]
        public string Provedor { get; set; }

        [JsonProperty(PropertyName = "tipo", Required = Required.Default)]
        public int Tipo { get; set; }

        [JsonProperty(PropertyName = "proprietario", Required = Required.Default)]
        public string Proprietario { get; set; }

        [JsonProperty(PropertyName = "observacao", Required = Required.Default)]
        public string Observacao { get; set; }
    }
}
