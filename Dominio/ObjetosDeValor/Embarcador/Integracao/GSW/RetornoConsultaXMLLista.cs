using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GSW
{
    public class RetornoConsultaXMLLista
    {
        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public long Codigo { get; set; }

        [JsonProperty(PropertyName = "chave", Required = Required.Default)]
        public string Chave { get; set; }

        [JsonProperty(PropertyName = "xml", Required = Required.Default)]
        public string XML { get; set; }
    }
}
