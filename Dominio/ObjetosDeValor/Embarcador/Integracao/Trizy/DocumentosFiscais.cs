using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class DocumentosFiscais
    {
        public string travel { get; set; }

        public string documentType { get; set; }

        public string file { get; set; }

        public string label { get; set; }

        public string description { get; set; }
    }
}
