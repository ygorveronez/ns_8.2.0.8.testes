using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Brado
{
    public class RequestCargaCancelamento
    {
        [JsonProperty("module")]
        public string Modulo { get; set; }

        [JsonProperty("operation")]
        public string Operacao { get; set; }

        [JsonProperty("parameters")]
        public ParametrosCargaCancelamento Parametros { get; set; }
    }
}
