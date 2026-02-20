using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GSW
{
    public class RetornoConsultaXML
    {
        [JsonProperty(PropertyName = "xmls", Required = Required.Default)]
        public RetornoConsultaXMLLista[] ListaXML { get; set; }

        [JsonProperty(PropertyName = "tipoXml", Required = Required.Default)]
        public int TipoXml { get; set; }

        [JsonProperty(PropertyName = "statusCode", Required = Required.Default)]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public string Mensagem { get; set; }
    }
}
