using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SAD
{
    public sealed class RequisicaoIntegracaoFinalizarAgenda
    {
        [JsonProperty(PropertyName = "numeroAgendaME", Required = Required.Default, Order = 1)]
        public string NumeroAgendamento { get; set; }
        
        [JsonProperty(PropertyName = "filialCarga", Required = Required.Default, Order = 2)]
        public string FilialCarga { get; set; }
    }
}
