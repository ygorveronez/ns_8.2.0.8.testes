using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Magalu
{
    public class EventoCodigo
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Codigo { get; set; }
    }
}
