using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Arquivei
{
    public class RetornoConsultaXMLArquivei
    {
        [JsonProperty(PropertyName = "Status.Code", Required = Required.Default)]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "Status.Message", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "Error", Required = Required.Default)]
        public string Erro { get; set; }

        [JsonProperty(PropertyName = "data", Required = Required.Default)]
        public RetornoConsultaXmlListaArquivei[] ListaXML { get; set; }

        [JsonProperty(PropertyName = "page.next", Required = Required.Default)]
        public string Proximo { get; set; }

        [JsonProperty(PropertyName = "page.previous", Required = Required.Default)]
        public string Anterior { get; set; }

        //[JsonProperty(PropertyName = "page", Required = Required.Default)]
        //public RetornoConsultaXmlListaRegistroArquivei[] Registro { get; set; }

        [JsonProperty(PropertyName = "count", Required = Required.Default)]
        public int Quantidade { get; set; }

        [JsonProperty(PropertyName = "signature", Required = Required.Default)]
        public string Assinatura { get; set; }
    }
}
