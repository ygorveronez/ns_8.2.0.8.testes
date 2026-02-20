using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class TrocaPedido
    {
        [JsonProperty(PropertyName = "cabecalho", Required = Required.Always)]
        public TrocaPedidoCabecalho Cabecalho { get; set; }

        [JsonProperty(PropertyName = "pedidoDePara", Required = Required.Always)]
        public TrocaPedidoDePara DePara { get; set; }
    }
}
