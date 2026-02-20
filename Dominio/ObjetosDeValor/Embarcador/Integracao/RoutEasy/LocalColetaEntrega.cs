using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class LocalColetaEntrega
    {
        [JsonProperty(PropertyName = "code")]
        public string CodigoCliente { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string NomeCliente { get; set; }

        [JsonProperty(PropertyName = "address")]
        public Endereco Endereco { get; set; }

        [JsonProperty(PropertyName = "constraints")]
        public PedidoRestricao Restricao { get; set; }
    }
}
