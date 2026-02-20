using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public class Pix
    {
        [JsonProperty(PropertyName = "tipo", Required = Required.Default)]
        public string Tipo { get; set; }

        [JsonProperty(PropertyName = "chave", Required = Required.Default)]
        public string Chave { get; set; }
    }
}
