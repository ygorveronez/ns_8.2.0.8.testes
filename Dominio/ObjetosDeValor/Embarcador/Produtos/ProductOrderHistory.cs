using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class ProductOrderHistory
    {
        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("historyItems")]
        public List<HistoryItem> HistoryItems { get; set; }
    }
}
