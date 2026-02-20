using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Italac
{
    public class DocumentoFatura
    {
        [JsonProperty(PropertyName = "numeroDocumento")]
        public int NumeroDocumento { get; set; }

        [JsonProperty(PropertyName = "tipoDocumento")]
        public string TipoDocumento { get; set; }

        [JsonProperty(PropertyName = "serieDocumento")]
        public string SerieDocumento { get; set; }

        [JsonProperty(PropertyName = "valorTotalDocumento")]
        public decimal ValorTotalDocumento { get; set; }

        [JsonProperty(PropertyName = "chaveCTE")]
        public string ChaveCTe { get; set; }

    }
}
