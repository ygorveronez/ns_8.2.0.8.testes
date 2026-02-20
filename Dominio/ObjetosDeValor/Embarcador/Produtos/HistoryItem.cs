using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class HistoryItem
    {
        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("materials")]
        public List<Material> Materials { get; set; }
    }
}
