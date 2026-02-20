using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte
{
    public sealed class RequisicaoTelhaNortePedido
    {
        [JsonProperty(PropertyName = "protocolo_pedido", Order = 1, Required = Required.Always)]
        public int ProtocoloPedido { get; set; }

        [JsonProperty(PropertyName = "numero_remessa", Order = 8, Required = Required.Always)]
        public string NumeroRemessa { get; set; }
    }
}