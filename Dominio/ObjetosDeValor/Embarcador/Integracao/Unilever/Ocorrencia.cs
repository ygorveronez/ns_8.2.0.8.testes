using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class Ocorrencia
    {
        [JsonProperty("occurrenceNumber")]
        public string NumeroOcorrencia { get; set; }
    }
}
