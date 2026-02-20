using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal
{
    public class ResponseHandshake : Response
    {
        [JsonProperty(PropertyName = "object")]
        public Handshake Handshake { get; set; }
    }
}
