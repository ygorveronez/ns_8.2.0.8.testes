using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.META.WhatsApp
{
    public class ParametrosComponente
    {
        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("text")]
        public string Texto { get; set; }

    }
}
