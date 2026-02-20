using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class IntegracaoCTeDocumento
    {
        [JsonProperty(PropertyName = "SAPDOCTYPE", Required = Required.Always)]
        public string TipoDocumentoSap { get; set; }

        [JsonProperty(PropertyName = "DOCNUMBER", Required = Required.Always)]
        public string NumeroDocumento { get; set; }

        [JsonProperty(PropertyName = "STAGE", Required = Required.Always)]
        public string Stage { get; set; }

        [JsonProperty(PropertyName = "FISDOCTYPE", Required = Required.Always)]
        public string TipoDocumentoFis { get; set; }

        [JsonProperty(PropertyName = "FISDOCKEY", Required = Required.Always)]
        public string ChaveDocumentoFis { get; set; }

        [JsonProperty(PropertyName = "REFDOCKEY", Required = Required.Always)]
        public string ChaveReferenciaDocumento { get; set; }

        [JsonProperty(PropertyName = "UNIDOCS", Required = Required.Always)]
        public string UniDocs { get; set; }

        [JsonProperty(PropertyName = "STATUS", Required = Required.Always)]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "DOCISSUANCE", Required = Required.Always)]
        public string EmissaoDocumento { get; set; }

        [JsonProperty(PropertyName = "BASE64FILE", Required = Required.Always)]
        public string ArquivoBase64 { get; set; }
    }
}
