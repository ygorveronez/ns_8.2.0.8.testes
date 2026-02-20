using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.BBC
{
    public class RetornoIntegrarPagamentoViagem
    {
        [JsonProperty(PropertyName = "mensagem", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "sucesso", Required = Required.Default)]
        public bool Sucesso { get; set; }

        [JsonProperty(PropertyName = "data", Required = Required.Default)]
        public RetornoDados Dados { get; set; }
    }
}
