using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52
{
    public class Login
    {
        [JsonProperty(PropertyName = "cpfcnpj", Required = Required.Default)]
        public string CpfCnpj { get; set; }

        [JsonProperty(PropertyName = "senha", Required = Required.Default)]
        public string Senha { get; set; }
    }
}
