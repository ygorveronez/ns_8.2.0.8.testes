using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Brado
{
    public class RequestLoginIntegracao
    {
        [JsonProperty("module")]
        public string Modulo { get; set; }

        [JsonProperty("operation")]
        public string Operacao { get; set; }

        [JsonProperty("parameters")]
        public ParametrosLoginIntegracao Parametros { get; set; }
    }
}
