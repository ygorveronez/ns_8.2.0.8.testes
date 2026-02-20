using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.META.WhatsApp
{
    public class ComponentesTemplate
    {
        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("format")]
        public string Formato { get; set; }

        [JsonProperty("text")]
        public string Texto { get; set; }

        [JsonProperty("example")]
        public ExemploVariavelParaTemplate ExemploVariavel { get; set; }

    }
}
