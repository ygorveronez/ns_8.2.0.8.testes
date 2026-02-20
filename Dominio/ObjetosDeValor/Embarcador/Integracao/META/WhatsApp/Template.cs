using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.META.WhatsApp
{
    public class Template
    {
        [JsonProperty("name")]
        public string Nome { get; set; }

        [JsonProperty("language")]
        public Idioma Idioma { get; set; }

        [JsonProperty("components")]
        public List<Componente> Componentes { get; set; }


    }
}
