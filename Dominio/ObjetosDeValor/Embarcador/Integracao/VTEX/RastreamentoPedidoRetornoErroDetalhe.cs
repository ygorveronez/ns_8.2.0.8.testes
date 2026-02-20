using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX
{
    public class RastreamentoPedidoRetornoErroDetalhe
    {
        [JsonProperty(PropertyName = "code", Required = Required.Default)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "exception", Required = Required.Default)]
        public string Excecao { get; set; }
    }
}
