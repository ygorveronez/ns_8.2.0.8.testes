using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52
{
    public class ColetaPedidoRota
    {
        [JsonProperty(PropertyName = "cd_identificador", Required = Required.Default)]
        public string Identificador { get; set; }

        [JsonProperty(PropertyName = "cd_tipo_rota", Required = Required.Default)]
        public string TipoRota { get; set; }

        [JsonProperty(PropertyName = "pontos", Required = Required.Default)]
        public string Pontos { get; set; }

        [JsonProperty(PropertyName = "polilinha", Required = Required.Default)]
        public string Polilinha { get; set; }
    }
}
