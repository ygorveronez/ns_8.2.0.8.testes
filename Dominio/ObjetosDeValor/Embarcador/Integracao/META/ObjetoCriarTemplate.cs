using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.META.WhatsApp
{
    public class ObjetoCriarTemplate
    {
        [JsonProperty("name")]
        public string Nome { get; set; }

        [JsonProperty("category")]
        public string Categoria { get; set; }

        [JsonProperty("allow_category_change")]
        public bool PermiteMudancaCategoria { get; set; }

        [JsonProperty("language")]
        public string Idioma { get; set; }

        [JsonProperty("components")]
        public List<ComponentesTemplate> Componentes { get; set; }

    }
}
