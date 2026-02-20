using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class OrdemEmbarquePedido
    {
        [JsonProperty(PropertyName = "empresaPedidoCNPJ", Required = Required.Always)]
        public string EmpresaCnpj { get; set; }

        [JsonProperty(PropertyName = "numeroBau", Required = Required.AllowNull)]
        public int NumeroBau { get; set; }

        [JsonProperty(PropertyName = "numeroPedido", Required = Required.Always)]
        public string NumeroPedido { get; set; }

        [JsonProperty(PropertyName = "motoristaCpf", Required = Required.Always)]
        public string MotoristaCpf { get; set; }

        [JsonProperty(PropertyName = "placaVeiculo", Required = Required.AllowNull)]
        public string PlacaVeiculo { get; set; }

        [JsonProperty(PropertyName = "protocoloTMSPedido", Required = Required.Always)]
        public int ProtocoloPedido { get; set; }
    }
}
