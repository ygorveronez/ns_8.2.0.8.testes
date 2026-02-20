using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Magalu
{
    public class Evento
    {
        [JsonProperty(PropertyName = "date", Required = Required.Always)]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.Always)]
        public EventoCodigo Status { get; set; }
    }
}
