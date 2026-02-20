using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class DOCUMENT
    {
        [JsonProperty("SAPDOCTYPE")]
        public string SAPDOCTYPE;

        [JsonProperty("DOCNUMBER")]
        public string NumeroCarga;

        [JsonProperty("STAGE")]
        public string STAGE;

        [JsonProperty("FISDOCTYPE")]
        public string FISDOCTYPE;

        [JsonProperty("FISDOCKEY")]
        public string FISDOCKEY;

        [JsonProperty("REFDOCKEY")]
        public string REFDOCKEY;

        [JsonProperty("UNIDOCS")]
        public string UNIDOCS;

        [JsonProperty("STATUS")]
        public string STATUS;

        [JsonProperty("DOCISSUANCE")]
        public string DOCISSUANCE;

        [JsonProperty("BASE64FILE")]
        public string BASE64FILE;
    }
}
