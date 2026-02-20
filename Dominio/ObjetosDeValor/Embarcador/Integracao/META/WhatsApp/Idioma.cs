using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.META.WhatsApp
{
    public class Idioma
    {
        [JsonProperty("code")]
        public string SiglaIdioma { get; set; }
    }
}
