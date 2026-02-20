using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class AlteracaoPedidoPedido
    {
        [JsonProperty(PropertyName = "empresaPedidoCNPJ", Required = Required.Always)]
        public string EmpresaCnpj { get; set; }

        [JsonProperty(PropertyName = "numeroPedido", Required = Required.Always)]
        public string NumeroPedido { get; set; }

        [JsonProperty(PropertyName = "protocoloTMSPedido", Required = Required.Always)]
        public int ProtocoloPedido { get; set; }
    }
}
