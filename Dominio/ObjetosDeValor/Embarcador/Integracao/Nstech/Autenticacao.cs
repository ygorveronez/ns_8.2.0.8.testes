using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech
{
    public class Autenticacao
    {
        [JsonProperty(PropertyName = "solicitante_id", Required = Required.Always)]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "solicitante_senha", Required = Required.Always)]
        public string Senha { get; set; }
    }
}
