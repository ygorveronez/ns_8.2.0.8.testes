using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz
{
    public class RespostaDadosContabilizacaoErro
    {
        [JsonProperty(PropertyName = "status", Required = Required.Default)]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "code", Required = Required.Default)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "instruction", Required = Required.Default)]
        public string Instrucao { get; set; }

        [JsonProperty(PropertyName = "detail", Required = Required.Default)]
        public string Detalhe { get; set; }

        [JsonProperty(PropertyName = "type", Required = Required.Default)]
        public string Tipo { get; set; }
    }
}
