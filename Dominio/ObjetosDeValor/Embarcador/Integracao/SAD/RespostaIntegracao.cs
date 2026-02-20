using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SAD
{
    public sealed class RespostaIntegracao
    {
        [JsonProperty(PropertyName = "agenda", Required = Required.Default)]
        public string SenhaAgendamento { get; set; }
        
        [JsonProperty(PropertyName = "mensagem", Required = Required.Default)]
        public string MensagemErro { get; set; }
    }
}
