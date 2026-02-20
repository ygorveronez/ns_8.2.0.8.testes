using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz
{
    public class RespostaRequisicaoVeiculoErro
    {
        [JsonProperty(PropertyName = "codigo", Required = Required.Default)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "mensagem", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "instrucao", Required = Required.Default)]
        public string Instrucao { get; set; }

        [JsonProperty(PropertyName = "detalhe", Required = Required.Default)]
        public string Detalhe { get; set; }

        [JsonProperty(PropertyName = "tipo", Required = Required.Default)]
        public string Tipo { get; set; }
    }
}
