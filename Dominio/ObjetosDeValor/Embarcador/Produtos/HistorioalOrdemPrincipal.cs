using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class HistorioalOrdemPrincipal
    {
        [JsonProperty("iDoc")]
        public int IDoc { get; set; }

        [JsonProperty("productOrderHistories")]
        public List<ProductOrderHistory> ProductOrderHistories { get; set; }

    }
}
