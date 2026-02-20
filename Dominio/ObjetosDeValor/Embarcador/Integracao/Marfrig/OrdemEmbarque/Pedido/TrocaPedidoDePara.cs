using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class TrocaPedidoDePara
    {
        [JsonProperty(PropertyName = "empresaPedidoDeCNPJ", Required = Required.Always)]
        public string EmpresaPedidoDeCnpj { get; set; }

        [JsonProperty(PropertyName = "empresaPedidoParaCNPJ", Required = Required.Always)]
        public string EmpresaPedidoParaCnpj { get; set; }

        [JsonProperty(PropertyName = "numeroPedidoDe", Required = Required.Always)]
        public string NumeroPedidoDe { get; set; }

        [JsonProperty(PropertyName = "numeroPedidoPara", Required = Required.Always)]
        public string NumeroPedidoPara { get; set; }

        [JsonProperty(PropertyName = "protocoloTMSPedidoDe", Required = Required.Always)]
        public int ProtocoloPedidoDe { get; set; }

        [JsonProperty(PropertyName = "protocoloTMSPedidoPara", Required = Required.Always)]
        public int ProtocoloPedidoPara { get; set; }
    }
}
