using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class AcrescimoDesconto
    {
        [JsonProperty(PropertyName = "success", Required = Required.Always)]
        public string Sucesso { get; set; }

        [JsonProperty(PropertyName = "code", Required = Required.Always)]
        public int Codigo { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "detail")]
        public Detalhe Detalhe { get; set; }

        [JsonProperty(PropertyName = "result")]
        public Resultado Resultado { get; set; }
    }
    public class Detalhe
    {
        [JsonProperty(PropertyName = "error")]
        public decimal Erro { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "stacktrace")]
        public string Stacktrace { get; set; }

        [JsonProperty(PropertyName = "cod_mensagem")]
        public string CodigoMensagem { get; set; }
    }
    public class Resultado
    {
        [JsonProperty(PropertyName = "message")]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "lancto_id")]
        public string Lancamento { get; set; }

        [JsonProperty(PropertyName = "valor")]
        public decimal Valor { get; set; }

    }
}
