using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX
{
    public class RastreamentoPedidoRetornoErro
    {
        [JsonProperty(PropertyName = "error", Required = Required.Default)]
        public RastreamentoPedidoRetornoErroDetalhe Erro { get; set; }
    }
}
