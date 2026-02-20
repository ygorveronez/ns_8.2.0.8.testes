using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Brado
{
    public class DocumentosCargaCancelamento
    {
        [JsonProperty("chave_documento")]
        public string ChaveDocumento { get; set; }

        [JsonProperty("cnpj_emissor")]
        public string EmissorCNJPJ { get; set; }
        
        [JsonProperty("num_documento")]
        public string NumeroDocumento { get; set; }

        [JsonProperty("serie")]
        public string Serie { get; set; }
    }
}
