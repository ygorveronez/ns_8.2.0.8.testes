using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class RemessaPedido
    {
        [JsonProperty("Remessa")]
        public string Remessa { get; set; }

        [JsonProperty("Protocolo")]
        public string Protocolo { get; set; }

        [JsonProperty("ChaveNota")]
        public string ChaveNota { get; set; }

        [JsonProperty("Num_cpfCnpj")]
        public string CpfCnpjCliente { get; set; }

        [JsonProperty("PedidoVTEX")]
        public string Pedido { get; set; }

        [JsonProperty("NumeroNFe")]
        public string NumeroNFe { get; set; }

        [JsonProperty("Tracking")]
        public string Tracking { get; set; }

        [JsonProperty("Filial")]
        public string Filial { get; set; }
    }
}
