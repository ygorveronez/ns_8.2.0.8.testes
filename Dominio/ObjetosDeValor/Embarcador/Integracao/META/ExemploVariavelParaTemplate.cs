using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.META.WhatsApp
{
    public class ExemploVariavelParaTemplate
    {

        [JsonProperty("header_text")]
        public List<dynamic> ConteudoVarivaelHeaderMensagem { get; set; }

        [JsonProperty("body_text")]
        public List<dynamic> ConteudoVarivaelBodyMensagem { get; set; }
    }
}
