using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa
{
    public class RequestMarisaExtra
    {
        [JsonProperty("_type")]
        public string TipoHistorico;
    }
}