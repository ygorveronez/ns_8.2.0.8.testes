using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto
{
    public class ListaPedidos
    {
        [JsonProperty("CodigoPedido")]
        public int CodigoPedido { get; set; }
    }
}
