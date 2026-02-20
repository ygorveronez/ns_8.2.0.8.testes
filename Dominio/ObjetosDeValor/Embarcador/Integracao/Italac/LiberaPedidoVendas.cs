using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Italac
{
    public class LiberaPedidoVendas
    {
        [JsonProperty(PropertyName = "cnpjembarcador")]
        public string CnpjRemetentePedido { get; set; }

        [JsonProperty(PropertyName = "nrpedido")]
        public string NumeroPedidoEmbarcador { get; set; }

        [JsonProperty(PropertyName = "usuariolib")]
        public string CpfUsuario { get; set; }
    }
}
