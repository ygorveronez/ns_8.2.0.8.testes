using Newtonsoft.Json;
using System.Collections.Generic;


namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class Material
    {
        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("item")]
        public string Item { get; set; } 

        [JsonProperty("materialDocumentYears")]
        public List<MaterialDocumentYear> MaterialDocumentYears { get; set; }
    }
}
