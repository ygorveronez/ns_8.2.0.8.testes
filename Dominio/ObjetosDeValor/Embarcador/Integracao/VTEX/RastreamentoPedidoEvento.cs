using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX
{
    public class RastreamentoPedidoEvento
    {
        [JsonProperty(PropertyName = "city", Required = Required.Always)]
        public string Cidade { get; set; }

        [JsonProperty(PropertyName = "state", Required = Required.Always)]
        public string Estado { get; set; }

        [JsonProperty(PropertyName = "description", Required = Required.Always)]
        public string Descricao { get; set; }

        [JsonProperty(PropertyName = "date", Required = Required.Always)]
        public string Data { get; set; }
    }
}
