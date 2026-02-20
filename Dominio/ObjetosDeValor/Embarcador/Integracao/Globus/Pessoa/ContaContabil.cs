using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public class ContaContabil
    {
        [JsonProperty(PropertyName = "planoContabil", Required = Required.Default)]
        public int PlanoContabil { get; set; }

        [JsonProperty(PropertyName = "contaContabil", Required = Required.Default)]
        public int NumeroContaContabil { get; set; }

        [JsonProperty(PropertyName = "contraPartida", Required = Required.Default)]
        public int ContraPartida { get; set; }
    }
}
