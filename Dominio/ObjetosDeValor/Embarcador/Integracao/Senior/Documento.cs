using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senior
{
    public class Documento
    {
        [JsonProperty(PropertyName = "type", Required = Required.Default)]
        public int Tipo { get; set; }

        [JsonProperty(PropertyName = "number", Required = Required.Default)]
        public string Numero { get; set; }
    }
}
