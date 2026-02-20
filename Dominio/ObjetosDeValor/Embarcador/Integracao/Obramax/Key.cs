using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class Key
    {
        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("data")]
        public string Dados { get; set; }
    }
}
