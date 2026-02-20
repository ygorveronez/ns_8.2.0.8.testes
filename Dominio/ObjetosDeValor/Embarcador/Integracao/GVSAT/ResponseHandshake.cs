using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT
{
    public class ResponseHandshake : Response
    {
        [JsonProperty(PropertyName = "object")]
        public Handshake Handshake { get; set; }
    }
}
