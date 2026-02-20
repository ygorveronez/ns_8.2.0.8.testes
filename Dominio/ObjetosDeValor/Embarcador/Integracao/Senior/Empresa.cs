using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senior
{
    public class Empresa
    {
        [JsonProperty(PropertyName = "code", Required = Required.Default)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public string Nome { get; set; }
    }
}
