using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class AlteracaoPedido
    {
        [JsonProperty(PropertyName = "cabecalho", Required = Required.Always)]
        public AlteracaoPedidoCabecalho Cabecalho { get; set; }

        [JsonProperty(PropertyName = "pedidos", Required = Required.Always)]
        public List<AlteracaoPedidoPedido> Pedidos { get; set; }
    }
}
