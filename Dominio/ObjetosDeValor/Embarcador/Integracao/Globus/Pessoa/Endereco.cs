using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public class Endereco
    {
        [JsonProperty(PropertyName = "cep", Required = Required.Default)]
        public string Cep { get; set; }

        [JsonProperty(PropertyName = "numeroEndereco", Required = Required.Default)]
        public int NumeroEndereco { get; set; }

        [JsonProperty(PropertyName = "complementoEndereco", Required = Required.Default)]
        public string ComplementoEndereco { get; set; }
    }
}
