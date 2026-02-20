using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class Documento
    {
        [JsonProperty("documentNumber")]
        public string NumeroDocumento { get; set; }

        [JsonProperty("documentTypeId")]
        public string IdTipoDocumento { get; set; }
    }
}
