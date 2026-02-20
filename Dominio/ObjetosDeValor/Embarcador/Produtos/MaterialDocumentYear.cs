using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class MaterialDocumentYear
    {
        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("materialItems")]
        public List<MaterialItem> MaterialItems { get; set; }
    }
}
