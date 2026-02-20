using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz
{
    public class RespostaRequisicaoVeiculo
    {
        [JsonProperty(PropertyName = "fault", Required = Required.Default)]
        public RespostaRequisicaoVeiculoErro Erro { get; set; }

        [JsonProperty(PropertyName = "return", Required = Required.Default)]
        public string RetornoSucesso { get; set; }
    }
}
