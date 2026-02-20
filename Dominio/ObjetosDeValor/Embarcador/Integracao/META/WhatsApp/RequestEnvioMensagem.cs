using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.META.WhatsApp
{
    public class RequestEnvioMensagem
    {
        [JsonProperty("messaging_product")]
        public string ProdutoEnvioMensagem { get; set; }

        [JsonProperty("to")]
        public string Para { get; set; }

        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("template")]
        public Template Template { get; set; }

    }
}
