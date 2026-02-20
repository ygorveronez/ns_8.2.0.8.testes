using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SAD
{
    public sealed class RequisicaoIntegracaoBuscarSenha
    {
        [JsonProperty(PropertyName = "numeroPedido", Required = Required.Default)]
        public string NumeroPedido { get; set; }

        [JsonProperty(PropertyName = "numeroAgendaME", Required = Required.Default, Order = 2)]
        public string NumeroAgendamento { get; set; }

        [JsonProperty(PropertyName = "dataEntrega", Required = Required.Default, Order = 4)]
        public string DataJanela { get; set; }

        [JsonProperty(PropertyName = "horaEntrega", Required = Required.Default, Order = 1)]
        public string HoraJanela { get; set; }

        [JsonProperty(PropertyName = "tipoCarga", Required = Required.Default, Order = 3)]
        public string TipoCarga { get; set; }

        [JsonProperty(PropertyName = "quantidade", Required = Required.Default)]
        public string Quantidade { get; set; }
        
        [JsonProperty(PropertyName = "filialCarga", Required = Required.Default)]
        public string FilialCarga { get; set; }
    }
}
