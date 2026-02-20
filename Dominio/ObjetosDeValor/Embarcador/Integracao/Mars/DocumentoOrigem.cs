using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class DocumentoOrigem
    {
        [JsonProperty("docType")]
        public string TipoDocumento { get; set; }

        [JsonProperty("key")]
        public string Chave { get; set; }

        [JsonProperty("series")]
        public string Serie { get; set; }

        [JsonProperty("number")]
        public string Numero { get; set; }
    }
}
