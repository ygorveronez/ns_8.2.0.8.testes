using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public class Documento
    {
        [JsonProperty(PropertyName = "tipo", Required = Required.Default)]
        public string Tipo { get; set; }

        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string Numero { get; set; }
    }
}
