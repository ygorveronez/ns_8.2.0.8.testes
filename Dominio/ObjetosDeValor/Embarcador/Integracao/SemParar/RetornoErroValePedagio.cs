using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar
{
    public class RetornoErroValePedagio
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("statusMessage")]
        public string MensagemStatus { get; set; }

        [JsonProperty("error")]
        public RetornoErro Erro { get; set; }

        [JsonProperty("timestamp")]
        public string DataHora { get; set; }

        [JsonProperty("path")]
        public string Caminho { get; set; }
    }

    public class RetornoErro
    {
        [JsonProperty("description")]
        public string Descricao { get; set; }

        [JsonProperty("validations")]
        public object Validacoes { get; set; }

        [JsonProperty("codeBusines")]
        public int CodigoNegocio { get; set; }
    }
}